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

/// <summary>The structural kind of a Concerto class declaration.</summary>
public enum DeclarationKind { Concept, Asset, Participant, Transaction, Event }

/// <summary>
/// Defines the structure (schema) of a composite Concerto type.
/// May have properties, a super-type, and an identifying field.
/// Mirrors ClassDeclaration in concerto-core.
/// </summary>
public sealed class ClassDeclaration : Declaration
{
    private readonly ReadOnlyCollection<Property> ownProperties;

    internal ClassDeclaration(
        ModelManager modelManager,
        string @namespace, string? version, string name,
        DeclarationKind kind, bool isAbstract, string? superTypeFqn,
        string? identifierFieldName, bool isExplicitlyIdentified,
        Func<ClassDeclaration, IEnumerable<Property>> propertiesFactory,
        IEnumerable<Decorator> decorators)
        : base(modelManager, @namespace, version, name, decorators)
    {
        Kind = kind;
        IsAbstractValue = isAbstract;
        SuperTypeFqn = superTypeFqn;
        IdentifierFieldName = identifierFieldName;
        IsExplicitlyIdentifiedValue = isExplicitlyIdentified;
        ownProperties = propertiesFactory(this).ToArray().AsReadOnly();
    }

    public DeclarationKind Kind { get; }

    public string? SuperTypeFqn { get; }

    public string? IdentifierFieldName { get; }

    private bool IsAbstractValue { get; }

    private bool IsExplicitlyIdentifiedValue { get; }

    public bool IsAbstract() => IsAbstractValue;

    public bool IsExplicitlyIdentified() => IsExplicitlyIdentifiedValue;

    public bool IsIdentified() => IdentifierFieldName is not null;

    public string? GetIdentifierFieldName() => IdentifierFieldName;

    public Property? GetOwnProperty(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return ownProperties.FirstOrDefault(property => string.Equals(property.Name, name, StringComparison.Ordinal));
    }

    public IReadOnlyCollection<Property> GetOwnProperties() => ownProperties;

    public string? GetSuperType() => SuperTypeFqn;

    public ClassDeclaration? GetSuperTypeDeclaration()
        => SuperTypeFqn is null ? null : ModelManager.GetType(SuperTypeFqn);

    public IReadOnlyCollection<ClassDeclaration> GetAssignableClassDeclarations()
        => ModelManager.FindAssignableClassDeclarations(this).ToArray();

    public IReadOnlyCollection<ClassDeclaration> GetDirectSubclasses()
        => ModelManager.FindDirectSubclasses(this).ToArray();

    public IReadOnlyCollection<ClassDeclaration> GetAllSuperTypeDeclarations()
    {
        var declarations = new List<ClassDeclaration>();
        var current = GetSuperTypeDeclaration();
        while (current is not null)
        {
            declarations.Add(current);
            current = current.GetSuperTypeDeclaration();
        }

        return declarations;
    }

    public Property? GetProperty(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return GetProperties().FirstOrDefault(property => string.Equals(property.Name, name, StringComparison.Ordinal));
    }

    public IReadOnlyCollection<Property> GetProperties()
    {
        var properties = new List<Property>();
        var superTypeDeclaration = GetSuperTypeDeclaration();
        if (superTypeDeclaration is not null)
        {
            properties.AddRange(superTypeDeclaration.GetProperties());
        }

        foreach (var property in ownProperties)
        {
            properties.RemoveAll(existing => existing.Name == property.Name);
            properties.Add(property);
        }

        return properties.AsReadOnly();
    }

    public bool IsAsset() => Kind == DeclarationKind.Asset;

    public bool IsParticipant() => Kind == DeclarationKind.Participant;

    public bool IsTransaction() => Kind == DeclarationKind.Transaction;

    public bool IsEvent() => Kind == DeclarationKind.Event;

    /// <summary>
    /// Returns true only when this is a Concept and not an Asset, Participant, Transaction, or Event.
    /// </summary>
    public bool IsConcept() => Kind == DeclarationKind.Concept;
}
