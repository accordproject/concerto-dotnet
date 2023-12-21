using Microsoft.ClearScript.V8;
using AccordProject.Concerto.Validate;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;

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
        var result =await validator.Validate(models, instance);
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
        var result = await validator.Validate(models, instance);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.ErrorMessage);
    }
}
