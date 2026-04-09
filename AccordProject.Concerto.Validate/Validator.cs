using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Meta = AccordProject.Concerto.Metamodel;

namespace AccordProject.Concerto.Validate;

public class Validator
{
    private static readonly JsonSerializerSettings _metamodelSettings = new()
    {
        Converters = { new AccordProject.Concerto.ConcertoConverterNewtonsoft() }
    };

    public Task<ValidationResult> Validate(string[] modelJsons, string instanceJson, ValidationOptions options)
    {
        try
        {
            // Build FQN -> ConceptDeclaration lookup across all provided models
            var declarationMap = new Dictionary<string, Meta.ConceptDeclaration>(StringComparer.Ordinal);
            foreach (var modelJson in modelJsons)
            {
                var model = JsonConvert.DeserializeObject<Meta.Model>(modelJson, _metamodelSettings)
                    ?? throw new InvalidOperationException("Model JSON deserialized to null.");
                foreach (var decl in model.Declarations ?? [])
                {
                    if (decl is Meta.ConceptDeclaration cd)
                        declarationMap[$"{model.Namespace}.{cd.Name}"] = cd;
                }
            }

            var instance = JObject.Parse(instanceJson);
            ValidateObject(instance, declarationMap, strict: options.Strict);

            var id = instance["$identifier"]?.Value<string>();
            return Task.FromResult(new ValidationResult
            {
                IsValid = true,
                Instance = instanceJson,
                Id = id,
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ValidationResult
            {
                IsValid = false,
                ErrorMessage = ex.Message,
            });
        }
    }

    public Task<string[]> GeAllReferencedNamespaces(string objectJson)
    {
        var fqns = CollectClassValues(JToken.Parse(objectJson));
        var namespaces = new HashSet<string>(StringComparer.Ordinal);
        foreach (var fqn in fqns)
        {
            var ns = ExtractNamespace(fqn);
            if (ns != null)
                namespaces.Add(ns);
        }
        return Task.FromResult(namespaces.ToArray());
    }

    // -- Validation helpers ---------------------------------------------------

    private static void ValidateObject(
        JObject obj,
        IReadOnlyDictionary<string, Meta.ConceptDeclaration> declarationMap,
        bool strict)
    {
        var fqn = obj["$class"]?.Value<string>()
            ?? throw new InvalidOperationException("JSON object is missing '$class'.");

        if (!declarationMap.TryGetValue(fqn, out var decl))
            throw new InvalidOperationException($"Type '{fqn}' is not defined in the provided models.");

        foreach (var prop in decl.Properties ?? [])
        {
            if (prop is null) continue;
            var token = obj[prop.Name];

            if (token == null || token.Type == JTokenType.Null)
            {
                if (!prop.IsOptional)
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{fqn}' is required but missing.");
                continue;
            }

            if (prop.IsArray)
            {
                if (token.Type != JTokenType.Array)
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{fqn}' must be an array.");
                foreach (var element in (JArray)token)
                    ValidatePropertyValue(element, prop, fqn, declarationMap, strict);
            }
            else
            {
                ValidatePropertyValue(token, prop, fqn, declarationMap, strict);
            }
        }
    }

    private static void ValidatePropertyValue(
        JToken token,
        Meta.Property prop,
        string ownerFqn,
        IReadOnlyDictionary<string, Meta.ConceptDeclaration> declarationMap,
        bool strict)
    {
        switch (prop)
        {
            case Meta.StringProperty:
                if (token.Type != JTokenType.String)
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{ownerFqn}' must be a String, got {token.Type}.");
                break;

            case Meta.BooleanProperty:
                if (token.Type != JTokenType.Boolean)
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{ownerFqn}' must be a Boolean, got {token.Type}.");
                break;

            case Meta.IntegerProperty:
            case Meta.LongProperty:
                if (token.Type != JTokenType.Integer)
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{ownerFqn}' must be an Integer, got {token.Type}.");
                break;

            case Meta.DoubleProperty:
                if (token.Type != JTokenType.Float && token.Type != JTokenType.Integer)
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{ownerFqn}' must be a Double, got {token.Type}.");
                break;

            case Meta.DateTimeProperty:
                if (token.Type != JTokenType.String ||
                    !DateTime.TryParse(token.Value<string>(), out _))
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{ownerFqn}' must be a valid DateTime string, got {token.Type}.");
                break;

            case Meta.ObjectProperty:
                if (token.Type != JTokenType.Object)
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{ownerFqn}' must be an Object, got {token.Type}.");
                ValidateObject((JObject)token, declarationMap, strict);
                break;

            case Meta.RelationshipProperty:
                // Relationships are serialised as strings (resource identifiers).
                if (token.Type != JTokenType.String)
                    throw new InvalidOperationException(
                        $"Property '{prop.Name}' on '{ownerFqn}' must be a relationship string, got {token.Type}.");
                break;
        }
    }

    // -- Namespace extraction helpers -----------------------------------------

    // Matches versioned FQNs: {namespace}@{major}.{minor}.{patch}.{TypeName}
    private static readonly Regex VersionedFqnRegex =
        new(@"^(.+)@(\d+\.\d+\.\d+)\.(.+)$", RegexOptions.Compiled);

    private static string? ExtractNamespace(string fqn)
    {
        var m = VersionedFqnRegex.Match(fqn);
        if (m.Success)
            // Return as "{name}.{version}" matching concerto-core ModelUtil behaviour
            return $"{m.Groups[1].Value}.{m.Groups[2].Value}";

        // Un-versioned FQN: strip the last dotted segment
        var lastDot = fqn.LastIndexOf('.');
        return lastDot > 0 ? fqn[..lastDot] : null;
    }

    private static IEnumerable<string> CollectClassValues(JToken token)
    {
        var results = new List<string>();
        var stack = new Stack<JToken>();
        stack.Push(token);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current is JObject obj)
            {
                foreach (var prop in obj.Properties())
                {
                    if (prop.Name == "$class" && prop.Value.Type == JTokenType.String)
                        results.Add(prop.Value.Value<string>()!);
                    else if (prop.Value.Type == JTokenType.Object || prop.Value.Type == JTokenType.Array)
                        stack.Push(prop.Value);
                }
            }
            else if (current is JArray arr)
            {
                foreach (var item in arr)
                    stack.Push(item);
            }
        }
        return results;
    }
}
