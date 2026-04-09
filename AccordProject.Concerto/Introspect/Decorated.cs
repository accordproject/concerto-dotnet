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

namespace AccordProject.Concerto;

/// <summary>
/// Base for any model element that may carry Concerto decorators.
/// Mirrors the Decorated class in concerto-core.
/// </summary>
public abstract class Decorated(IEnumerable<Decorator> decorators)
{
    private readonly ReadOnlyCollection<Decorator> decorators = decorators.ToArray().AsReadOnly();

    public IReadOnlyCollection<Decorator> GetDecorators()
    {
        return decorators;
    }

    public Decorator? GetDecorator(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return decorators.FirstOrDefault(decorator => string.Equals(decorator.Name, name, StringComparison.Ordinal));
    }
}
