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

/// <summary>
/// Base class for all Concerto type declarations (classes and enums).
/// Conceptually owned by a ModelFile.
/// Mirrors the Declaration class in concerto-core.
/// </summary>
public abstract class Declaration(
    ModelManager modelManager,
    string @namespace,
    string? version,
    string name,
    IEnumerable<Decorator> decorators) : Decorated(decorators)
{
    private ModelFile? modelFile;

    public ModelManager ModelManager { get; } = modelManager;

    public string Namespace { get; } = @namespace;

    public string? Version { get; } = version;

    public string Name { get; } = name;

    public string FullyQualifiedName => new ConcertoType { Namespace = Namespace, Version = Version, Name = Name }.ToString();

    public ModelFile GetModelFile()
    {
        return modelFile ?? throw new InvalidOperationException($"Model file for '{FullyQualifiedName}' has not been attached.");
    }

    internal void AttachModelFile(ModelFile file)
    {
        modelFile = file;
    }
}
