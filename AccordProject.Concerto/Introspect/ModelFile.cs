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
/// Groups all declarations that belong to a single Concerto namespace.
/// Mirrors ModelFile in concerto-core.
/// </summary>
public sealed class ModelFile : Decorated
{
    private readonly List<Declaration> declarations = new();
    private readonly ReadOnlyCollection<Declaration> readonlyDeclarations;

    internal ModelFile(ModelManager modelManager, string @namespace, string? version)
        : base(Array.Empty<Decorator>())
    {
        ModelManager = modelManager;
        Namespace = @namespace;
        Version = version;
        readonlyDeclarations = declarations.AsReadOnly();
    }

    public ModelManager ModelManager { get; }

    public string Namespace { get; }

    public string? Version { get; }

    public string GetNamespace() => Namespace;

    public string? GetVersion() => Version;

    public IReadOnlyCollection<Declaration> GetAllDeclarations() => readonlyDeclarations;

    public IReadOnlyCollection<ClassDeclaration> GetClassDeclarations()
        => declarations.OfType<ClassDeclaration>().ToArray();

    public IReadOnlyCollection<ClassDeclaration> GetAssetDeclarations()
        => declarations.OfType<ClassDeclaration>().Where(static d => d.IsAsset()).ToArray();

    public IReadOnlyCollection<ClassDeclaration> GetParticipantDeclarations()
        => declarations.OfType<ClassDeclaration>().Where(static d => d.IsParticipant()).ToArray();

    public IReadOnlyCollection<ClassDeclaration> GetTransactionDeclarations()
        => declarations.OfType<ClassDeclaration>().Where(static d => d.IsTransaction()).ToArray();

    public IReadOnlyCollection<ClassDeclaration> GetEventDeclarations()
        => declarations.OfType<ClassDeclaration>().Where(static d => d.IsEvent()).ToArray();

    public IReadOnlyCollection<ClassDeclaration> GetConceptDeclarations()
        => declarations.OfType<ClassDeclaration>().Where(static d => d.IsConcept()).ToArray();

    public IReadOnlyCollection<EnumDeclaration> GetEnumDeclarations()
        => declarations.OfType<EnumDeclaration>().ToArray();

    internal void AddDeclaration(Declaration declaration)
    {
        declarations.RemoveAll(existing => existing.FullyQualifiedName == declaration.FullyQualifiedName);
        declarations.Add(declaration);
        declarations.Sort(static (left, right) => StringComparer.Ordinal.Compare(left.FullyQualifiedName, right.FullyQualifiedName));
    }
}
