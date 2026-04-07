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

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
public class TypeAttribute : Attribute
{
    public string Namespace;
    public string? Version;
    public string Name;

    public ConcertoType ToType()
    {
        return new ConcertoType()
        {
            Namespace = Namespace,
            Version = Version,
            Name = Name
        };
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class IdentifierAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class DecoratorAttribute : Attribute
{
    public DecoratorAttribute(string name, params object?[] arguments)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Decorator name is required.", nameof(name));
        }

        Name = name;
        Arguments = arguments ?? Array.Empty<object?>();
    }

    public string Name { get; }

    public object?[] Arguments { get; }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RelationshipAttribute : Attribute
{
}