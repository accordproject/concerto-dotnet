using System;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;
using System.IO;
using Newtonsoft.Json;

namespace AccordProject.Concerto.Validate
{
    public class Validator
    {
        private INodeJSService nodeJSService { get; set; }
        private string script { get; set;  } 
        public Validator(INodeJSService nodeJSService) {
            this.nodeJSService = nodeJSService;
            this.script = File.ReadAllText("concerto-validate/dist/validate.js");
        }

        public async Task<ValidationResult> Validate(string[] modelJson, string instanceJson)
        {
            var models = "[" + string.Join(",\n", modelJson) + "]";
            var args = new[] { models, instanceJson };
            var cacheIdentifier = "validate";
            return await nodeJSService.InvokeFromStringAsync<ValidationResult>(script, cacheIdentifier, "validateInstance", args);
	    }
    }
}

