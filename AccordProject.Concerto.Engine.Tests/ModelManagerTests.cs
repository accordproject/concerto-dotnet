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

using AccordProject.Concerto.Engine;
using Xunit;

namespace AccordProject.Concerto.Engine.Tests;

public class ModelManagerTests
{
    private const string PersonModel = """
    {
      "$class": "concerto.metamodel@1.0.0.Model",
      "namespace": "org.acme.hr@1.0.0",
      "imports": [
        { "$class": "concerto.metamodel@1.0.0.ImportType", "name": "Address", "namespace": "org.acme.address@1.0.0" }
      ],
      "declarations": [
        {
          "$class": "concerto.metamodel@1.0.0.ConceptDeclaration",
          "name": "Person",
          "isAbstract": false,
          "identified": { "$class": "concerto.metamodel@1.0.0.IdentifiedBy", "name": "email" },
          "properties": [
            { "$class": "concerto.metamodel@1.0.0.StringProperty", "name": "email", "isArray": false, "isOptional": false },
            { "$class": "concerto.metamodel@1.0.0.StringProperty", "name": "name", "isArray": false, "isOptional": false },
            { "$class": "concerto.metamodel@1.0.0.IntegerProperty", "name": "age", "isArray": false, "isOptional": true },
            {
              "$class": "concerto.metamodel@1.0.0.ObjectProperty", "name": "address", "isArray": false, "isOptional": true,
              "type": { "$class": "concerto.metamodel@1.0.0.TypeIdentifier", "name": "Address" }
            }
          ]
        }
      ]
    }
    """;

    private const string AddressModel = """
    {
      "$class": "concerto.metamodel@1.0.0.Model",
      "namespace": "org.acme.address@1.0.0",
      "declarations": [
        {
          "$class": "concerto.metamodel@1.0.0.ConceptDeclaration",
          "name": "Address",
          "isAbstract": false,
          "properties": [
            { "$class": "concerto.metamodel@1.0.0.StringProperty", "name": "city", "isArray": false, "isOptional": false }
          ]
        }
      ]
    }
    """;

    private static ModelManager LoadPeople()
    {
        var manager = new ModelManager();
        // Out of import order on purpose: the batch is resolved as a whole.
        manager.AddModels(PersonModel, AddressModel);
        return manager;
    }

    [Fact]
    public void EngineReportsItsVersion()
    {
        Assert.False(string.IsNullOrEmpty(ModelManager.EngineVersion));
    }

    [Fact]
    public void AddModelsReturnsNamespacesAndExposesDeclarations()
    {
        using var manager = new ModelManager();
        var namespaces = manager.AddModels(PersonModel, AddressModel);

        Assert.Equal(new[] { "org.acme.hr@1.0.0", "org.acme.address@1.0.0" }, namespaces);
        Assert.Contains("org.acme.hr@1.0.0.Person", manager.DeclarationNames);
        Assert.Contains("org.acme.address@1.0.0.Address", manager.DeclarationNames);
    }

    [Fact]
    public void ValidInstanceHasNoDiagnostics()
    {
        using var manager = LoadPeople();
        var report = manager.Validate("""
        {
          "$class": "org.acme.hr@1.0.0.Person",
          "email": "ada@example.com",
          "name": "Ada Lovelace",
          "age": 36,
          "address": { "$class": "org.acme.address@1.0.0.Address", "city": "London" }
        }
        """);

        Assert.True(report.IsValid);
        Assert.Empty(report.Diagnostics);
    }

    [Theory]
    [InlineData("""{ "$class": "org.acme.hr@1.0.0.Person", "email": "grace@example.com", "name": 42 }""", "/name", "TYPE_VIOLATION")]
    [InlineData("""{ "$class": "org.acme.hr@1.0.0.Person", "email": "grace@example.com" }""", "/name", "MISSING_REQUIRED_PROPERTY")]
    [InlineData("""{ "$class": "org.acme.hr@1.0.0.Person", "email": "g@example.com", "name": "G", "address": { "$class": "org.acme.address@1.0.0.Address" } }""", "/address/city", "MISSING_REQUIRED_PROPERTY")]
    public void InvalidInstanceReportsAPointedDiagnostic(string instance, string pointer, string code)
    {
        using var manager = LoadPeople();
        var report = manager.Validate(instance);

        Assert.False(report.IsValid);
        var diagnostic = Assert.Single(report.Diagnostics);
        Assert.Equal(pointer, diagnostic.Pointer);
        Assert.Equal(code, diagnostic.Code);
        Assert.Equal(Severity.Error, diagnostic.Severity);
        Assert.NotEmpty(diagnostic.Message);
    }

    [Fact]
    public void ValidateOrThrowRaisesAValidationException()
    {
        using var manager = LoadPeople();
        var ex = Assert.Throws<ConcertoException>(() => manager.ValidateOrThrow("""
        { "$class": "org.acme.hr@1.0.0.Person", "email": "grace@example.com", "name": 42 }
        """));

        Assert.Equal("Validation", ex.Kind);
        Assert.False(string.IsNullOrEmpty(ex.Code));
        Assert.Contains("name", ex.Message);
    }

    [Fact]
    public void UnknownKeysAreRejectedWithStructuredDetailsUnderStrict()
    {
        using var manager = LoadPeople();
        const string withTypo = """
        { "$class": "org.acme.hr@1.0.0.Person", "email": "grace@example.com", "name": "Grace Hopper", "adge": 85 }
        """;

        // concerto-core rejects undeclared properties by default too...
        var lenient = Assert.Throws<ConcertoException>(() => manager.ValidateOrThrow(withTypo));
        Assert.Equal("Validation", lenient.Kind);
        Assert.Empty(lenient.Details);

        // ...but the accordproject/concerto#1273 strict preset adds structured details.
        var strict = Assert.Throws<ConcertoException>(() => manager.ValidateOrThrow(withTypo, ValidationOptions.Strict));
        Assert.Equal("Validation", strict.Kind);
        var detail = Assert.Single(strict.Details);
        Assert.Equal("UNKNOWN_PROPERTY", detail.Code);

        var report = manager.Validate(withTypo, ValidationOptions.Strict);
        Assert.Contains(report.Diagnostics, d => d.Pointer == "/adge" && d.Code == "UNDECLARED_FIELD");
    }

    [Fact]
    public void RequiredNullIsRejectedUnderStrict()
    {
        using var manager = LoadPeople();
        const string withNull = """
        { "$class": "org.acme.hr@1.0.0.Person", "email": "grace@example.com", "name": null }
        """;

        var ex = Assert.Throws<ConcertoException>(() => manager.ValidateOrThrow(withNull, ValidationOptions.Strict));
        Assert.Equal("TYPE_VIOLATION", Assert.Single(ex.Details).Code);
    }

    [Fact]
    public void UnknownTypeIsReported()
    {
        using var manager = LoadPeople();
        var report = manager.Validate("""{ "$class": "org.acme.hr@1.0.0.Robot" }""");

        Assert.False(report.IsValid);
        Assert.Contains("Robot", Assert.Single(report.Diagnostics).Message);
    }

    [Fact]
    public void InvalidModelIsRejectedAndTheBatchRolledBack()
    {
        using var manager = new ModelManager();
        // Person imports Address, which is not in the batch.
        var ex = Assert.Throws<ConcertoException>(() => manager.AddModels(PersonModel));

        Assert.Equal("IllegalModel", ex.Kind);
        Assert.DoesNotContain("org.acme.hr@1.0.0.Person", manager.DeclarationNames);
    }

    [Fact]
    public void MalformedJsonIsAnInvalidArgument()
    {
        using var manager = LoadPeople();
        var ex = Assert.Throws<ConcertoException>(() => manager.Validate("{ not json"));

        Assert.Equal("InvalidArgument", ex.Kind);
    }

    [Fact]
    public void ValidationIsSafeToRunConcurrently()
    {
        using var manager = LoadPeople();
        Parallel.For(0, 200, i =>
        {
            var report = manager.Validate($$"""
            { "$class": "org.acme.hr@1.0.0.Person", "email": "p{{i}}@example.com", "name": "P{{i}}", "age": {{i}} }
            """);
            Assert.True(report.IsValid);
        });
    }

    [Fact]
    public void DisposedManagerThrows()
    {
        var manager = LoadPeople();
        manager.Dispose();
        manager.Dispose();

        Assert.Throws<ObjectDisposedException>(() => manager.Validate("{}"));
    }

    // The fixtures the Node-hosted AccordProject.Concerto.Validate tests use,
    // run through the native engine instead.

    [Fact]
    public void ExistingValidFixtureValidates()
    {
        using var manager = new ModelManager();
        manager.AddModels(File.ReadAllText("Fixtures/testModelAST.json"));

        var report = manager.Validate(File.ReadAllText("Fixtures/testInstanceValid.json"), ValidationOptions.Strict);
        Assert.True(report.IsValid);
    }

    [Fact]
    public void ExistingInvalidFixtureIsRejected()
    {
        using var manager = new ModelManager();
        manager.AddModels(File.ReadAllText("Fixtures/testModelAST.json"));

        var report = manager.Validate(File.ReadAllText("Fixtures/testInstanceInvalid.json"), ValidationOptions.Strict);
        Assert.False(report.IsValid);
        var diagnostic = Assert.Single(report.Diagnostics);
        Assert.Equal("/value", diagnostic.Pointer);
        Assert.NotEmpty(diagnostic.Message);
    }
}
