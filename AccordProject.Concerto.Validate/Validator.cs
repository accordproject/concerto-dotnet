using System;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;
using System.IO;
using Newtonsoft.Json;
using AccordProject.Concerto.Validate.concertovalidate;
using Microsoft.Extensions.Options;

namespace AccordProject.Concerto.Validate
{
    public class Validator
    {
        private INodeJSService nodeJSService { get; set; }
        private const string cacheIdentifier = "validate";

        private string script { get; set;  } 
        public Validator(INodeJSService nodeJSService) {
            this.nodeJSService = nodeJSService;
            this.script = File.ReadAllText("concerto-validate/dist/validate.js");
        }

        public async Task<ValidationResult> Validate(string[] modelJson, string instanceJson, ValidationOptions options)
        {
            var models = "[" + string.Join(",\n", modelJson) + "]";
            var args = new object[] { models, instanceJson, options};
            var cacheIdentifier = "validate";
            return await nodeJSService.InvokeFromStringAsync<ValidationResult>(script, cacheIdentifier, "validateInstance", args);
	    }

        public async Task<string[]> GetAllReferencedTypeNames(string objectJson)
        {
            var args = new object[] { objectJson };
            return await nodeJSService.InvokeFromStringAsync<string[]>(script, cacheIdentifier, "getAllNeededNamespaces", args);

        }
    }
}