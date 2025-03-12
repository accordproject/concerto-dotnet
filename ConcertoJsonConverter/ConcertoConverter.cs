/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Concerto.Models.concerto;

namespace Concerto.Serialization;

public static class JsonSubtypesExtension {   
    public static bool IsSubType(this Type s, Type t) {
        for (var u = s; u != null; u = u.BaseType){
            if (u == t){
                return true;
            }
        }
        return false;
    }
}

public class ConcertoConverter : JsonConverterFactory
{
    public override bool CanConvert(Type type) => typeof(Concept).IsAssignableFrom(type);

    // We need a Factory to produce all of the generic-typed variants of the converters
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return (JsonConverter)Activator.CreateInstance(typeof(ConcertoJsonConverter<>).MakeGenericType(typeToConvert))!;
    }

    // We make this private because we only want consumers to use the factory
    private class ConcertoJsonConverter<T>: JsonConverter<T> where T: Concept {
        
        // We dynamically build a dictionary of T and any sub-types with the 
        // fully qualified type names as keys. This is protected against deserialization exploits
        // because we restrict instances of this class to to types that extend Concept,
        // i.e. an attacker cannot instantiated arbitrary objects. 
        public ConcertoJsonConverter(){
            foreach(var type in Assembly.GetExecutingAssembly().GetTypes().Where(
                p => typeof(T).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract
            )){
                typeDictionary[type.FullName] = type;
            }   
        }

        private static Dictionary<string, Type> typeDictionary = new Dictionary<string, Type>();

        private object deserializeWithGenericType(Type type, String rawText, object options)
        {
            string[] expectedTypes = [typeof(string).FullName, typeof(JsonSerializerOptions).FullName];
            var method = typeof(JsonSerializer).GetMethods()
                .Where(x => x.Name == nameof(JsonSerializer.Deserialize))
                .FirstOrDefault(
                    x => x.IsGenericMethod && x.GetParameters().Select(p => p.ParameterType.FullName)
                        .SequenceEqual(expectedTypes));
            return method.MakeGenericMethod(type).Invoke(null, new object[] { rawText, options});
        }

        private CustomAttributeData getJsonPropertyNameAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributesData()
                .Where(attribute => attribute.AttributeType.Name == "JsonPropertyNameAttribute")
                .FirstOrDefault();
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {           
            writer.WriteStartObject();
            
            foreach (var propertyInfo in value.GetType().GetProperties()
                .Where(pi => pi.GetGetMethod() != null)
            ){
                var propertyName = propertyInfo.Name;
                var propertyValue = propertyInfo.GetGetMethod().Invoke(value, null);
    
                // Skip null values according to options
                if (propertyValue == null 
                    && options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull
                ){
                    continue;
                }
    
                // Apply property name transformations
                CustomAttributeData jsonPropertyNameAttribute = getJsonPropertyNameAttribute(propertyInfo);
                if (jsonPropertyNameAttribute != null){
                    propertyName = (string) jsonPropertyNameAttribute
                        .ConstructorArguments.FirstOrDefault().Value;
                }

                writer.WritePropertyName
                    (options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);

                // Recursively serialize using the property value. 
                // This forces use of the instance type rather than the declared type in the base class)
                JsonSerializer.Serialize(writer, propertyValue, options);
            };

            writer.WriteEndObject();
        }

        public override T Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Only JSON Objects can be deserialized with ConcertoConverter.");
            }

            // Peek ahead at the object
            JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
            var jsonObject = jsonDocument.RootElement;

            // All Concerto documents must have a `$class` property
            if (!jsonObject.TryGetProperty("$class", out var clazz))
            {
                throw new JsonException("JSON Object is missing `$class` property.");
            }

            // Find the type from the dictionary that corresponds to the $class discriminator
            typeDictionary.TryGetValue("Concerto.Models." + clazz.GetString(), out Type declaredType);
            if (declaredType == null)
            {
                throw new JsonException("Type definition `" + clazz + "` not found.");
            }

            // Allow a more specific type to be provided instead of the type from the C# class, i.e. support polymorphic deserialization
            if (declaredType != objectType){
                if (declaredType.IsSubType(objectType)){
                    objectType = declaredType;
                } else {
                    throw new JsonException("Invalid type declaration. `" + declaredType + "` is not a valid subtype of the expected `"+ objectType +"`.");   
                }
            }

            // Create an empty object
            T concept = (T) Activator.CreateInstance(objectType);

            foreach (var item in jsonObject.EnumerateObject())
            {
                PropertyInfo property = objectType.GetProperties(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                ).Where(p => {
                    // Apply property name transformations
                    CustomAttributeData jsonPropertyNameAttribute = getJsonPropertyNameAttribute(p);
                    if (jsonPropertyNameAttribute != null) {
                        string attributeValue = (string) jsonPropertyNameAttribute
                        .ConstructorArguments.FirstOrDefault().Value;
                        return attributeValue == item.Name;
                    }
                    return p.Name == item.Name;
                })
                .FirstOrDefault();                

                if (property == null){
                    throw new JsonException("Property "+item.Name+" is not found.");
                }

                if (!property.CanWrite){
                    continue;
                }

                object element = deserializeWithGenericType(property.PropertyType, item.Value.GetRawText(), options);
                property.SetValue(concept, element, null);
            }
            return concept;
        }
    }
}