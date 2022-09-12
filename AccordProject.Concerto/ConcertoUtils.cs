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
}