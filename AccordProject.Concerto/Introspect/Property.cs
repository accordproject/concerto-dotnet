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
/// Represents an attribute (field or relationship) of a ClassDeclaration.
/// Mirrors the Property class in concerto-core.
/// </summary>
public sealed class Property : Decorated
{
    private readonly string fullyQualifiedTypeName;

    internal Property(
        ClassDeclaration parent,
        string name,
        string typeName,
        string fullyQualifiedTypeName,
        bool isOptional,
        bool isArray,
        bool isPrimitive,
        bool isTypeEnum,
        bool isRelationship,
        IEnumerable<Decorator> decorators)
        : base(decorators)
    {
        Parent = parent;
        Name = name;
        TypeName = typeName;
        this.fullyQualifiedTypeName = fullyQualifiedTypeName;
        IsOptionalValue = isOptional;
        IsArrayValue = isArray;
        IsPrimitiveValue = isPrimitive;
        IsTypeEnumValue = isTypeEnum;
        IsRelationshipValue = isRelationship;
    }

    public ClassDeclaration Parent { get; }

    public string Name { get; }

    public string TypeName { get; }

    public ClassDeclaration GetParent() => Parent;

    public bool IsOptional() => IsOptionalValue;

    public bool IsArray() => IsArrayValue;

    public bool IsTypeEnum() => IsTypeEnumValue;

    public bool IsPrimitive() => IsPrimitiveValue;

    public bool IsRelationship() => IsRelationshipValue;

    public string GetFullyQualifiedTypeName() => fullyQualifiedTypeName;

    public string GetFullyQualifiedName() => $"{Parent.FullyQualifiedName}.{Name}";

    public string GetNamespace() => Parent.Namespace;

    private bool IsOptionalValue { get; }

    private bool IsArrayValue { get; }

    private bool IsPrimitiveValue { get; }

    private bool IsTypeEnumValue { get; }

    private bool IsRelationshipValue { get; }
}
