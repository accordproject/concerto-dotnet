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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ConcertoConverterNewtonsoft : JsonConverter
{
    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanConvert(Type t) => t.IsAssignableTo(typeof(Concept));

    public override void WriteJson(JsonWriter writer, Object? value, JsonSerializer serializer)
    {
        // The default Newtonsoft behaviour is acceptable.
        // This method will never be called as CanWrite is always false.
        throw new NotImplementedException();
    }

    public override Object ReadJson(JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonException("Only JSON Objects can be deserialized with ConcertoConverterNewtonsoft.");
        }

        // Peek ahead at the object
        var jsonObject = JObject.Load(reader);

        // All Concerto documents must have a `$class` property
        string? clazz = jsonObject["$class"]?.Value<string>();
        if (clazz == null)
        {
            throw new JsonException("JSON Object is missing `$class` property.");
        }

        // Find the type from the dictionary that corresponds to the $class discriminator
        var declaredType = ConcertoTypeDictionary.Instance.ResolveType(clazz);
        if (declaredType == null)
        {
            throw new JsonException("Type definition `" + clazz + "` not found.");
        }

        // Allow a more specified type to be provided instead of the type from the C# class
        Type actualType;
        if (objectType == declaredType)
        {
            actualType = objectType;
        }
        else if (declaredType.IsAssignableTo(objectType))
        {
            // The declared type in the JSON is a subtype of the object type in the .NET class.
            actualType = declaredType;
        }
        else
        {
            throw new JsonException("Invalid type declaration. `" + declaredType + "` is not a valid subtype of the expected `" + objectType + "`.");
        }

        var target = Activator.CreateInstance(actualType);
        if (target == null)
        {
            throw new JsonException("Failed to create instance of `" + actualType + "`.");
        }
        serializer.Populate(jsonObject.CreateReader(), target);
        return target;
    }
}