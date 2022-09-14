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

namespace AccordProject.Concerto.Tests;

using System;
using AccordProject.Concerto.Metamodel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class ConcertoConverterNewtonsoftMetamodelTests
{
    [Fact]
    public void Roundtrip()
    {
        var carVIN = new StringProperty()
        {
            Name = "vin"
        };

        var carConcept = new ConceptDeclaration()
        {
            Name = "Car",
            IsAbstract = false,
            Properties = new StringProperty[1] { carVIN }
        };

        var carModel = new Model()
        {
            Namespace = "org.acme",
            Declarations = new ConceptDeclaration[1] { carConcept },
        };

        var jsonString = JsonConvert.SerializeObject(carModel);
        Model model2 = JsonConvert.DeserializeObject<Model>(jsonString);
        ConceptDeclaration car2 = (ConceptDeclaration) model2.Declarations[0];
        StringProperty prop = (StringProperty)((ConceptDeclaration)car2).Properties[0];
        Assert.Equal(prop._Class,"concerto.metamodel@1.0.0.StringProperty");
    }

    [Fact]
    public void Patent()
    {
        string jsonString = System.IO.File.ReadAllText(@"./../../../data/patent.json");
        Model model = JsonConvert.DeserializeObject<Model>(jsonString);
        // var jsonString2 = JsonConvert.SerializeObject(model, options);
        // Console.WriteLine(jsonString2);
    }
}