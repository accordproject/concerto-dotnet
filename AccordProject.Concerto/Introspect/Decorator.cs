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

using System.Collections.ObjectModel;
using System.Reflection;

namespace AccordProject.Concerto;

/// <summary>
/// Represents a single Concerto decorator and its arguments.
/// Mirrors the Decorator class in concerto-core.
/// </summary>
public sealed class Decorator(string name, IEnumerable<object?> arguments)
{
    public string Name { get; } = name;

    public IReadOnlyList<object?> Arguments { get; } = arguments.ToArray().AsReadOnly();

    internal static IReadOnlyCollection<Decorator> FromMember(MemberInfo member)
    {
        return member.GetCustomAttributes<DecoratorAttribute>(inherit: true)
            .Select(attribute => new Decorator(attribute.Name, attribute.Arguments))
            .ToArray();
    }
}
