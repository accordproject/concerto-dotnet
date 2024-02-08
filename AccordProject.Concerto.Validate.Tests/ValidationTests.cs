using Microsoft.ClearScript.V8;
using AccordProject.Concerto.Validate;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using AccordProject.Concerto.Validate.concertovalidate;

namespace AccordProject.Concerto.Validate.Tests;

public class ValidationTests
{

    [Fact]
    public async void GivenValidModelWhenValidateCalledThenIsValidShouldBeTrue()
    {

        var model = File.ReadAllText("Fixtures/testModelAST.json");
        var instance = File.ReadAllText("Fixtures/testInstanceValid.json");
        var services = new ServiceCollection();
        services.AddNodeJS();
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        INodeJSService nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();
        

        var validator = new Validator(nodeJSService);
        String[] models = { model };
        var options = new ValidationOptions { EnableMapType = true, Strict = true };
        var result =await validator.Validate(models, instance, options);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async void GivenInvalidModelWhenValidateCalledThenIsValidShouldBeFalseAndMessageShouldBePresent()
    {

        var model = File.ReadAllText("Fixtures/testModelAST.json");
        var instance = File.ReadAllText("Fixtures/testInstanceInvalid.json");
        var services = new ServiceCollection();
        services.AddNodeJS();
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        INodeJSService nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();


        var validator = new Validator(nodeJSService);
        String[] models = { model };
        var options = new ValidationOptions { EnableMapType = true, Strict = true };
        var result = await validator.Validate(models, instance, options);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.ErrorMessage);
    }

    [Fact]
    public async void GivenObjectReturnListOfTypes()
    {
        var instance = File.ReadAllText("Fixtures/testInstanceWithImports.json");
        var services = new ServiceCollection();
        services.AddNodeJS();
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        INodeJSService nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();


        var validator = new Validator(nodeJSService);
        var result = await validator.GetAllReferencedTypeNames(instance);

        Assert.NotEmpty(result);
        Assert.Equal(3, result.Length);
        Assert.Contains("com.example.e2e.withImports.1.0.0", result);
        Assert.Contains("com.example.e2e.withExtendedConcept.1.0.0", result);
        Assert.Contains("com.example.e2e.withAConceptToBeImported.1.0.0", result);
    }
}
