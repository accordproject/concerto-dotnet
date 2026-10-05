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

// Validates JSON instances against a Concerto model with the native
// concerto-rust engine. No Node.js process is involved.
//
//   dotnet run --project samples/ValidateInstance

using System.Diagnostics;
using AccordProject.Concerto.Engine;

// A model, as its JSON AST (concerto.metamodel@1.0.0.Model). This is what
// `concerto parse` produces from CTO:
//
//   namespace org.acme.hr@1.0.0
//   concept Person identified by email {
//     o String email
//     o String name
//     o Integer age optional
//   }
const string personModel = """
{
  "$class": "concerto.metamodel@1.0.0.Model",
  "namespace": "org.acme.hr@1.0.0",
  "declarations": [
    {
      "$class": "concerto.metamodel@1.0.0.ConceptDeclaration",
      "name": "Person",
      "isAbstract": false,
      "identified": { "$class": "concerto.metamodel@1.0.0.IdentifiedBy", "name": "email" },
      "properties": [
        { "$class": "concerto.metamodel@1.0.0.StringProperty", "name": "email", "isArray": false, "isOptional": false },
        { "$class": "concerto.metamodel@1.0.0.StringProperty", "name": "name", "isArray": false, "isOptional": false },
        { "$class": "concerto.metamodel@1.0.0.IntegerProperty", "name": "age", "isArray": false, "isOptional": true }
      ]
    }
  ]
}
""";

Console.WriteLine($"concerto-ffi {ModelManager.EngineVersion}");

using var models = new ModelManager();
models.AddModels(personModel);

var instances = new (string Label, string Json)[]
{
    ("valid", """{ "$class": "org.acme.hr@1.0.0.Person", "email": "ada@example.com", "name": "Ada Lovelace", "age": 36 }"""),
    ("wrong type", """{ "$class": "org.acme.hr@1.0.0.Person", "email": "grace@example.com", "name": 42 }"""),
    ("missing field", """{ "$class": "org.acme.hr@1.0.0.Person", "email": "grace@example.com" }"""),
    ("required null", """{ "$class": "org.acme.hr@1.0.0.Person", "email": "grace@example.com", "name": null }"""),
};

// 1. Collect-all validation: a report of diagnostics, each with a JSON Pointer.
//    Validation is always strict, as in Concerto v5.
foreach (var (label, json) in instances)
{
    var report = models.Validate(json);
    Console.WriteLine($"{label,-14} valid={report.IsValid}");
    foreach (var d in report.Diagnostics)
    {
        Console.WriteLine($"    {d.Pointer,-8} [{d.Code}] {d.Message}");
    }
}

// 2. First-error validation, as concerto-core's Serializer.fromJSON throws.
try
{
    models.ValidateOrThrow(instances[1].Json);
}
catch (ConcertoException e)
{
    Console.WriteLine($"ValidateOrThrow -> {e.Kind} ({e.Code}): {e.Message}");
}

// 3. A rough throughput figure for the managed -> native round trip.
const int iterations = 10_000;
var stopwatch = Stopwatch.StartNew();
for (var i = 0; i < iterations; i++)
{
    models.Validate(instances[0].Json);
}
stopwatch.Stop();
Console.WriteLine($"{iterations} validations in {stopwatch.ElapsedMilliseconds} ms " +
                  $"({stopwatch.Elapsed.TotalMicroseconds / iterations:F1} us each)");
