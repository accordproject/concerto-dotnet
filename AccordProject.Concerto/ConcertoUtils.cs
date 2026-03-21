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

namespace AccordProject.Concerto;

using System.Reflection;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions; 


/// <summary>
/// This class represents a Concerto type, for example org.example@1.2.3.Foo.
/// </summary>
public struct ConcertoNamespace
{
    public string Namespace { get; init; }
    public string? Version { get; init; }

    public override string ToString()
    {
        if (Version != null)
        {
            return $"{Namespace}@{Version}";
        }
        return Namespace;
    }
}

public struct ConcertoType
{
    public string Namespace { get; init; }
    public string? Version { get; init; }
    public string Name { get; init; }

    public override string ToString()
    {
        if (Version != null)
        {
            return $"{Namespace}@{Version}.{Name}";
        }
        return $"{Namespace}.{Name}";
    }
}

/// <summary>
/// This class provides static utility methods.
/// </summary>
public class ConcertoUtils
{
    public static ConcertoNamespace ParseNamespace(string ns)
    {
        string parsedNamespace;
        string? parsedVersion = null;
        int i = ns.IndexOf("@");
        if (i != -1)
        {
            parsedNamespace = ns[..i];
            parsedVersion =  ns[(i + 1)..];
        }
        else
        {
            parsedNamespace = ns;
        }
        if (parsedNamespace.Length == 0 || parsedVersion?.Length == 0)
        {
            throw new Exception($"Invalid namespace \"{ns}\"");
        }
        return new ConcertoNamespace()
        {
            Namespace = parsedNamespace,
            Version = parsedVersion
        };
    }

    public static ConcertoType ParseType(string fqn)
    {
        int i = fqn.LastIndexOf(".");
        if (i == -1)
        {
            throw new Exception($"Invalid fully qualified name \"{fqn}\"");
        }
        var namespaceAndVersion = fqn[..i];
        var name = fqn[(i + 1)..];
        if (name.Length == 0)
        {
            throw new Exception($"Invalid fully qualified name \"{fqn}\"");
        }
        string ns;
        string? version = null;
        int j = namespaceAndVersion.IndexOf("@");
        if (j != -1)
        {
            ns = namespaceAndVersion[..j];
            version = namespaceAndVersion[(j + 1)..];
        }
        else
        {
            ns = namespaceAndVersion;
        }
        if (ns.Length == 0 || version?.Length == 0)
        {
            throw new Exception($"Invalid fully qualified name \"{fqn}\"");
        }
        return new ConcertoType() { Namespace = ns, Version = version, Name = name };
    }

    public static bool HasIdentifier(Concept concept)
    {
        var type = concept.GetType();
        var identifierProperty = FindIdentifierProperty(type);
        return identifierProperty is not null;
    }

    public static string? GetIdentifier(Concept concept)
    {
        var type = concept.GetType();
        var identifierProperty = FindIdentifierProperty(type);
        if (identifierProperty is null)
        {
            return null;
        }
        return identifierProperty.GetValue(concept) as string;
    }

    private static PropertyInfo? FindIdentifierProperty(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var identifierProperty = properties.FirstOrDefault(property => property.GetCustomAttributes(typeof(IdentifierAttribute), false).Length > 0);
        if (identifierProperty is not null)
        {
            return identifierProperty;
        }
        var baseType = type.BaseType;
        if (baseType is null)
        {
            return null;
        }
        return FindIdentifierProperty(baseType);
    }

    private static readonly Regex ID_REGEX = new Regex(
        @"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4})(?:\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\u200C|\u200D)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    public static string NormalizeIdentifier(object? identifier, int truncateLength = -1)
    {
        if (identifier is null) return "null";
        if (identifier is not string) throw new Exception("Unsupported identifier type");
        var result = (string)identifier;
        if (result.Length == 0) throw new Exception("Unexpected error: empty identifier");

        result = Regex.Replace(result, @"^\p{Nd}", "_$0");

        result = Regex.Replace(result, @"[-‐−.@#:;><|/\\]", "_");
        result = Regex.Replace(result, @"\s", "_");
        result = Regex.Replace(result, @"[\u200C\u200D]", "_");

        result = Regex.Replace(result, @"(?:[\uD800-\uDBFF][\uDC00-\uDFFF])", m => EscapeCodePointPair(m.Value));

        result = Regex.Replace(
            result,
            @"(?!\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\u200C|\u200D|\\u[0-9A-Fa-f]{4})(.)",
            m => EscapeCodePoints(m.Value),
            RegexOptions.Singleline
        );

        result = Regex.Replace(result, @"[\uD800-\uDFFF]", m => EscapeCodePoints(m.Value));

        if (truncateLength > 0 && result.Length >= truncateLength)
        {
            result = result[..truncateLength];
        }

        if (!ID_REGEX.IsMatch(result))
        {
            throw new Exception($"Unexpected error. Not able to escape identifier '{result}'.");
        }

        return result;
    }

    private static string EscapeCodePoints(string input)
    {
        var sb = new StringBuilder();
        foreach (var rune in input.EnumerateRunes())
        {
            sb.Append('_');
            sb.Append(rune.Value.ToString("x", CultureInfo.InvariantCulture));
        }
        return sb.ToString();
    }

    private static string EscapeCodePointPair(string pair)
    {
        var cp = char.ConvertToUtf32(pair, 0);
        return "_" + cp.ToString("x", CultureInfo.InvariantCulture);
    }   
}