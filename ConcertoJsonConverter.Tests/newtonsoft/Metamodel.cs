
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
using Xunit;
using Concerto.Models.concerto.metamodel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConcertoJsonConverterTests.Newtonsoft;

[Collection("Sequential")]
public class Metamodel
{

    JsonSerializerSettings options = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters =
        {
            new StringEnumConverter(),
            new Concerto.Serialization.Newtonsoft.ConcertoConverter(),
        }
    };

    [Fact]
    public void Roundtrip()
    {
        var carVIN = new StringProperty()
        {
            name = "vin"
        };

        var carConcept = new ConceptDeclaration()
        {
            name = "Car",
            isAbstract = false,
            properties = new StringProperty[1] { carVIN }
        };

        var carModel = new Model()
        {
            _namespace = "org.acme",
            declarations = new ConceptDeclaration[1] { carConcept },
        };

        var jsonString = JsonConvert.SerializeObject(carModel, options);
        Model model2 = JsonConvert.DeserializeObject<Model>(jsonString, options);
        ConceptDeclaration car2 = (ConceptDeclaration) model2.declarations[0];
        StringProperty prop = (StringProperty)((ConceptDeclaration)car2).properties[0];
        Assert.Equal(prop._class,"concerto.metamodel.StringProperty");
    }

    [Fact]
    public void Patent()
    {
        string jsonString = System.IO.File.ReadAllText(@"./../../../data/patent.json");
        Model model = JsonConvert.DeserializeObject<Model>(jsonString, options);
        // var jsonString2 = JsonConvert.SerializeObject(model, options);
        // Console.WriteLine(jsonString2);
    }
}