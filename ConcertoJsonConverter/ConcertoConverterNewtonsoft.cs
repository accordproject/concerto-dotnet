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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Concerto.Models.concerto;

namespace Concerto.Serialization.Newtonsoft;

public static class JsonSubtypesExtension
{
    public static bool IsSubType(this Type s, Type t)
    {
        for (var u = s; u != null; u = u.BaseType)
        {
            if (u == t)
            {
                return true;
            }
        }
        return false;
    }
}

public class ConcertoConverter : JsonConverter
{

    [ThreadStatic]
    private static bool silentWrite;

    [ThreadStatic]
    private static bool silentRead;

    private static Dictionary<string, Type> typeDictionary = new Dictionary<string, Type>();

    public ConcertoConverter()
    {
        RegisterConceptTypes();
    }

    public static void RegisterConceptTypes()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(
            p => typeof(Concept).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract
        ))
        {
            typeDictionary[type.FullName] = type;
        }
    }

    public override bool CanConvert(Type t)
    {
        return typeof(Concept).IsAssignableFrom(t);
    }

    public sealed override bool CanWrite
    {
        get
        {
            var canWrite = !silentWrite;
            silentWrite = false;
            return canWrite;
        }
    }

    public sealed override bool CanRead
    {
        get
        {
            var canRead = !silentRead;
            silentRead = false;
            return canRead;
        }
    }

    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
    {
        silentWrite = true;
        serializer.Serialize(writer, value);
    }

    public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
    {
        silentRead = true;
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonException("Only JSON Objects can be deserialized with ConcertoConverter.");
        }

        // Peek ahead at the object
        JObject jsonObject;
        jsonObject = JObject.Load(reader);
        reader = jsonObject.CreateReader();

        // All Concerto documents must have a `$class` property
        string clazz = jsonObject["$class"]?.Value<string>();
        if (clazz == null)
        {
            throw new JsonException("JSON Object is missing `$class` property.");
        }

        // Find the type from the dictionary that corresponds to the $class discriminator
        typeDictionary.TryGetValue("Concerto.Models." + clazz, out Type declaredType);
        if (declaredType == null)
        {
            throw new JsonException("Type definition `" + clazz + "` not found.");
        }

        // Allow a more specified type to be provided instead of the type from the C# class
        if (declaredType != objectType)
        {
            if (declaredType.IsSubType(objectType))
            {
                objectType = declaredType;
            }
            else
            {
                throw new JsonException("Invalid type declaration. `" + declaredType + "` is not a valid subtype of the expected `" + objectType + "`.");
            }
        }
        return serializer.Deserialize(reader, objectType);
    }

    public static T Deserialize<T>(string s) where T : class
    {
        return JsonConvert.DeserializeObject(s, typeof(T)) as T;
    }

    public static string Serialize<T>(T value) where T : class
    {
        return JsonConvert.SerializeObject(value);
    }
}