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

namespace AccordProject.Concerto;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// System.Text.Json converter factory for Concerto concepts.
/// </summary>
public class ConcertoConverterFactorySystem : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Concept).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return (JsonConverter?)Activator.CreateInstance(typeof(ConcertoJsonConverterSystem<>).MakeGenericType(typeToConvert));
    }

    private class ConcertoJsonConverterSystem<T> : JsonConverter<T> where T : Concept
    {
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var propertyInfo in value.GetType().GetProperties().Where(pi => pi.GetGetMethod() != null))
            {
                var propertyName = GetSerializedPropertyName(propertyInfo, options);
                var propertyValue = propertyInfo.GetGetMethod()!.Invoke(value, null);

                if (propertyValue == null && options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull)
                {
                    continue;
                }

                writer.WritePropertyName(propertyName);
                JsonSerializer.Serialize(writer, propertyValue, options);
            }

            writer.WriteEndObject();
        }

        public override T? Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Only JSON Objects can be deserialized with ConcertoConverterFactorySystem, current token is {reader.TokenType}.");
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            var jsonObject = jsonDocument.RootElement;

            if (!jsonObject.TryGetProperty("$class", out var clazz))
            {
                throw new JsonException("JSON Object is missing `$class` property.");
            }

            var className = clazz.GetString();
            if (className == null)
            {
                throw new JsonException("JSON Object is missing `$class` property.");
            }

            var declaredType = ConcertoTypeDictionary.Instance.ResolveType(className);
            if (declaredType == null)
            {
                throw new JsonException("Type definition `" + className + "` not found.");
            }

            var actualType = objectType;
            if (declaredType != objectType)
            {
                if (objectType.IsAssignableFrom(declaredType))
                {
                    actualType = declaredType;
                }
                else
                {
                    throw new JsonException("Invalid type declaration. `" + declaredType + "` is not a valid subtype of the expected `" + objectType + "`.");
                }
            }

            var concept = Activator.CreateInstance(actualType)
                ?? throw new JsonException("Failed to create instance of `" + actualType + "`.");

            foreach (var item in jsonObject.EnumerateObject())
            {
                var property = actualType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(p => string.Equals(GetPropertyName(p), item.Name, StringComparison.Ordinal));

                if (property == null)
                {
                    throw new JsonException("Property " + item.Name + " is not found.");
                }

                if (!property.CanWrite)
                {
                    continue;
                }

                var deserialized = JsonSerializer.Deserialize(item.Value.GetRawText(), property.PropertyType, options);
                property.SetValue(concept, deserialized);
            }

            return (T)concept;
        }

        private static string GetSerializedPropertyName(PropertyInfo property, JsonSerializerOptions options)
        {
            var propertyName = GetPropertyName(property);
            return options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
        }

        private static string GetPropertyName(PropertyInfo property)
        {
            var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            return jsonPropertyName?.Name ?? property.Name;
        }
    }
}