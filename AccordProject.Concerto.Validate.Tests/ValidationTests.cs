namespace AccordProject.Concerto.Validate.Tests;

public class ValidationTests
{
    [Fact]
    public async Task GivenValidModelWhenValidateCalledThenIsValidShouldBeTrue()
    {
        var model = File.ReadAllText("Fixtures/testModelAST.json");
        var instance = File.ReadAllText("Fixtures/testInstanceValid.json");

        var validator = new Validator();
        var options = new ValidationOptions { EnableMapType = true, Strict = true };
        var result = await validator.Validate([model], instance, options);
        Assert.True(result.IsValid, result.ErrorMessage);
    }

    [Fact]
    public async Task GivenInvalidModelWhenValidateCalledThenIsValidShouldBeFalseAndMessageShouldBePresent()
    {
        var model = File.ReadAllText("Fixtures/testModelAST.json");
        var instance = File.ReadAllText("Fixtures/testInstanceInvalid.json");

        var validator = new Validator();
        var options = new ValidationOptions { EnableMapType = true, Strict = true };
        var result = await validator.Validate([model], instance, options);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.ErrorMessage!);
    }

    [Fact]
    public async Task GivenObjectReturnListOfTypes()
    {
        var instance = File.ReadAllText("Fixtures/testInstanceWithImports.json");

        var validator = new Validator();
        var result = await validator.GeAllReferencedNamespaces(instance);

        Assert.NotEmpty(result);
        Assert.Equal(3, result.Length);
        Assert.Contains("com.example.e2e.withImports.1.0.0", result);
        Assert.Contains("com.example.e2e.withExtendedConcept.1.0.0", result);
        Assert.Contains("com.example.e2e.withAConceptToBeImported.1.0.0", result);
    }
}
