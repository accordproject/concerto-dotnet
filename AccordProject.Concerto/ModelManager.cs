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

using System.Collections;
using System.Reflection;

namespace AccordProject.Concerto;

/// <summary>
/// Manages Concerto model declarations loaded from CLR assemblies.
/// The canonical entry point for the introspection API.
/// Mirrors ModelManager / BaseModelManager in concerto-core.
/// </summary>
public sealed class ModelManager
{
    // Primitive CLR type → Concerto type name mapping used when resolving property types.
    private static readonly NullabilityInfoContext NullabilityContext = new();
    private static readonly IReadOnlyDictionary<Type, string> PrimitiveTypeNames = new Dictionary<Type, string>
    {
        [typeof(string)] = "String",
        [typeof(bool)] = "Boolean",
        [typeof(int)] = "Integer",
        [typeof(short)] = "Integer",
        [typeof(byte)] = "Integer",
        [typeof(sbyte)] = "Integer",
        [typeof(ushort)] = "Integer",
        [typeof(uint)] = "Long",
        [typeof(long)] = "Long",
        [typeof(ulong)] = "Long",
        [typeof(float)] = "Double",
        [typeof(double)] = "Double",
        [typeof(decimal)] = "Double",
        [typeof(DateTime)] = "DateTime",
        [typeof(DateTimeOffset)] = "DateTime",
    };

    private readonly Dictionary<string, ClassDeclaration> classDeclarations = new(StringComparer.Ordinal);
    private readonly Dictionary<string, EnumDeclaration> enumDeclarations = new(StringComparer.Ordinal);
    // CLR enum type → registered FQN; populated during Phase 2 so property FQNs are fully resolved in Phase 3.
    private readonly Dictionary<Type, string> enumFqnsByRuntimeType = new();
    private readonly Dictionary<string, ModelFile> modelFiles = new(StringComparer.Ordinal);
    private readonly HashSet<Assembly> registeredAssemblies = new();
    private readonly object syncRoot = new();
    private readonly Introspector introspector;

    /// <summary>Initialises a ModelManager that scans all assemblies already loaded in the current AppDomain.</summary>
    public ModelManager()
        : this(AppDomain.CurrentDomain.GetAssemblies())
    {
    }

    /// <summary>Initialises a ModelManager that scans only the supplied assemblies.</summary>
    public ModelManager(IEnumerable<Assembly> assemblies)
    {
        introspector = new Introspector(this);
        foreach (var assembly in assemblies)
        {
            RegisterAssembly(assembly);
        }
    }

    public Introspector GetIntrospector() => introspector;

    /// <summary>
    /// Scans an assembly and registers all Concerto types it contains.
    /// Safe to call with the same assembly more than once; subsequent calls are no-ops.
    /// </summary>
    public void RegisterAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        lock (syncRoot)
        {
            if (!registeredAssemblies.Add(assembly))
            {
                return;
            }
        }

        var loadableTypes = GetLoadableTypes(assembly).ToArray();
        var candidateEnums = new Dictionary<Type, ConcertoNamespace>();

        // Phase 1: walk class properties to infer namespaces for enum types that carry no TypeAttribute.
        foreach (var type in loadableTypes.Where(IsConcertoClassType))
        {
            var concertoNamespace = GetTypeAttribute(type)!.ToType();
            foreach (var property in GetDeclaredModelProperties(type))
            {
                var propertyType = UnwrapPropertyType(property.PropertyType);
                if (propertyType.IsEnum)
                {
                    candidateEnums.TryAdd(propertyType, new ConcertoNamespace
                    {
                        Namespace = concertoNamespace.Namespace,
                        Version = concertoNamespace.Version
                    });
                }
            }
        }

        // Phase 2: register enums first so their FQNs are available when class properties are resolved.
        foreach (var enumType in loadableTypes.Where(static type => type.IsEnum))
        {
            var typeAttribute = GetTypeAttribute(enumType);
            if (typeAttribute is not null)
            {
                RegisterEnumDeclaration(enumType, new ConcertoNamespace
                {
                    Namespace = typeAttribute.Namespace,
                    Version = typeAttribute.Version
                });
                continue;
            }

            if (candidateEnums.TryGetValue(enumType, out var concertoNamespace))
            {
                RegisterEnumDeclaration(enumType, concertoNamespace);
            }
        }

        // Phase 3: register class declarations (all enum FQNs are now known).
        foreach (var type in loadableTypes.Where(IsConcertoClassType))
        {
            RegisterClassDeclaration(type);
        }
    }

    /// <summary>
    /// Registers a single CLR type. The type must either be a Concerto class (subclassing Concept
    /// with a TypeAttribute) or a CLR enum with a TypeAttribute.
    /// </summary>
    public void RegisterType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type.IsEnum)
        {
            var typeAttribute = GetTypeAttribute(type)
                ?? throw new InvalidOperationException($"Cannot register enum '{type.FullName}' without a {nameof(TypeAttribute)}.");
            RegisterEnumDeclaration(type, new ConcertoNamespace
            {
                Namespace = typeAttribute.Namespace,
                Version = typeAttribute.Version
            });
            return;
        }

        if (!IsConcertoClassType(type))
        {
            throw new InvalidOperationException($"Cannot register '{type.FullName}' because it is not a Concerto type.");
        }

        RegisterClassDeclaration(type);
    }

    public IReadOnlyCollection<ModelFile> GetModelFiles()
        => modelFiles.Values.OrderBy(modelFile => modelFile.GetNamespace(), StringComparer.Ordinal).ToArray();

    public ModelFile? GetModelFile(string @namespace)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(@namespace);
        modelFiles.TryGetValue(@namespace, out var modelFile);
        return modelFile;
    }

    public IEnumerable<string> GetNamespaces()
        => modelFiles.Keys.OrderBy(static item => item, StringComparer.Ordinal).ToArray();

    public ClassDeclaration? GetType(string fullyQualifiedTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullyQualifiedTypeName);
        classDeclarations.TryGetValue(fullyQualifiedTypeName, out var declaration);
        return declaration;
    }

    public EnumDeclaration? GetEnum(string fullyQualifiedTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullyQualifiedTypeName);
        enumDeclarations.TryGetValue(fullyQualifiedTypeName, out var declaration);
        return declaration;
    }

    public IReadOnlyCollection<ClassDeclaration> GetClassDeclarations()
        => classDeclarations.Values.OrderBy(d => d.FullyQualifiedName, StringComparer.Ordinal).ToArray();

    public IReadOnlyCollection<EnumDeclaration> GetEnumDeclarations()
        => enumDeclarations.Values.OrderBy(d => d.FullyQualifiedName, StringComparer.Ordinal).ToArray();

    internal IEnumerable<ClassDeclaration> FindAssignableClassDeclarations(ClassDeclaration declaration)
        => classDeclarations.Values
            .Where(candidate => IsAssignableTo(candidate, declaration))
            .OrderBy(candidate => candidate.FullyQualifiedName, StringComparer.Ordinal)
            .ToArray();

    internal IEnumerable<ClassDeclaration> FindDirectSubclasses(ClassDeclaration declaration)
        => classDeclarations.Values
            .Where(candidate => candidate.SuperTypeFqn == declaration.FullyQualifiedName)
            .OrderBy(candidate => candidate.FullyQualifiedName, StringComparer.Ordinal)
            .ToArray();

    // -------------------------------------------------------------------------
    // Private helpers – all CLR reflection extraction is contained here.
    // -------------------------------------------------------------------------

    private bool IsAssignableTo(ClassDeclaration candidate, ClassDeclaration target)
    {
        var current = (ClassDeclaration?)candidate;
        while (current is not null)
        {
            if (current.FullyQualifiedName == target.FullyQualifiedName)
            {
                return true;
            }

            current = current.GetSuperTypeDeclaration();
        }

        return false;
    }

    private void RegisterClassDeclaration(Type runtimeType)
    {
        var attr = GetTypeAttribute(runtimeType)
            ?? throw new InvalidOperationException($"Type '{runtimeType.FullName}' is missing a {nameof(TypeAttribute)}.");
        var kind = ComputeKind(runtimeType);
        var superTypeFqn = ComputeSuperTypeFqn(runtimeType);
        var identifierProp = FindIdentifierProperty(runtimeType, declaredOnly: false);
        var identifierFieldName = identifierProp is null ? null : GetSerializedPropertyName(identifierProp);
        var isExplicitlyIdentified = FindIdentifierProperty(runtimeType, declaredOnly: true) is not null;
        var declaration = new ClassDeclaration(
            this,
            attr.Namespace, attr.Version, attr.Name,
            kind, runtimeType.IsAbstract, superTypeFqn,
            identifierFieldName, isExplicitlyIdentified,
            parent => GetDeclaredModelProperties(runtimeType).Select(pi => CreateProperty(parent, pi)),
            Decorator.FromMember(runtimeType));
        AddDeclaration(declaration);
    }

    private void RegisterEnumDeclaration(Type runtimeType, ConcertoNamespace concertoNamespace)
    {
        var declaration = new EnumDeclaration(
            this,
            concertoNamespace.Namespace, concertoNamespace.Version, runtimeType.Name,
            Enum.GetNames(runtimeType),
            Decorator.FromMember(runtimeType));
        enumFqnsByRuntimeType[runtimeType] = declaration.FullyQualifiedName;
        AddDeclaration(declaration);
    }

    private Property CreateProperty(ClassDeclaration parent, PropertyInfo pi)
    {
        var unwrapped = UnwrapPropertyType(pi.PropertyType);
        return new Property(
            parent,
            GetSerializedPropertyName(pi),
            ComputeTypeName(unwrapped),
            ComputeFullyQualifiedTypeName(pi, parent),
            ComputeIsOptional(pi),
            ComputeIsArray(pi.PropertyType),
            PrimitiveTypeNames.ContainsKey(unwrapped),
            unwrapped.IsEnum,
            pi.GetCustomAttribute<RelationshipAttribute>(inherit: true) is not null,
            Decorator.FromMember(pi));
    }

    private string ComputeFullyQualifiedTypeName(PropertyInfo pi, ClassDeclaration parent)
    {
        var propertyType = UnwrapPropertyType(pi.PropertyType);
        if (PrimitiveTypeNames.TryGetValue(propertyType, out var primitiveTypeName))
        {
            return primitiveTypeName;
        }

        if (propertyType.IsEnum)
        {
            return enumFqnsByRuntimeType.TryGetValue(propertyType, out var fqn)
                ? fqn
                : new ConcertoType { Namespace = parent.Namespace, Version = parent.Version, Name = propertyType.Name }.ToString();
        }

        var typeAttribute = propertyType.GetCustomAttribute<TypeAttribute>(inherit: false);
        if (typeAttribute is not null)
        {
            return typeAttribute.ToType().ToString();
        }

        return propertyType.FullName ?? propertyType.Name;
    }

    private void AddDeclaration(Declaration declaration)
    {
        switch (declaration)
        {
            case ClassDeclaration classDeclaration:
                classDeclarations[classDeclaration.FullyQualifiedName] = classDeclaration;
                break;
            case EnumDeclaration enumDeclaration:
                enumDeclarations[enumDeclaration.FullyQualifiedName] = enumDeclaration;
                break;
        }

        var modelFile = GetOrCreateModelFile(declaration.Namespace, declaration.Version);
        modelFile.AddDeclaration(declaration);
        declaration.AttachModelFile(modelFile);
    }

    private ModelFile GetOrCreateModelFile(string @namespace, string? version)
    {
        if (!modelFiles.TryGetValue(@namespace, out var modelFile))
        {
            modelFile = new ModelFile(this, @namespace, version);
            modelFiles.Add(@namespace, modelFile);
        }

        return modelFile;
    }

    private static IEnumerable<PropertyInfo> GetDeclaredModelProperties(Type runtimeType)
        => runtimeType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static property => property.GetIndexParameters().Length == 0)
            .Where(static property => GetSerializedPropertyName(property) != "$class")
            .OrderBy(static property => property.MetadataToken);

    internal static string GetSerializedPropertyName(PropertyInfo property)
    {
        var jsonProperty = property.GetCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>(inherit: true);
        return !string.IsNullOrWhiteSpace(jsonProperty?.PropertyName) ? jsonProperty!.PropertyName! : property.Name;
    }

    internal static Type UnwrapPropertyType(Type propertyType)
    {
        var nullableType = Nullable.GetUnderlyingType(propertyType);
        if (nullableType is not null)
        {
            return nullableType;
        }

        if (propertyType != typeof(string) && propertyType != typeof(byte[]) && typeof(IEnumerable).IsAssignableFrom(propertyType))
        {
            if (propertyType.IsArray)
            {
                return propertyType.GetElementType() ?? propertyType;
            }

            if (propertyType.IsGenericType)
            {
                return propertyType.GetGenericArguments()[0];
            }
        }

        return propertyType;
    }

    private static DeclarationKind ComputeKind(Type runtimeType)
    {
        if (typeof(Asset).IsAssignableFrom(runtimeType)) return DeclarationKind.Asset;
        if (typeof(Participant).IsAssignableFrom(runtimeType)) return DeclarationKind.Participant;
        if (typeof(Transaction).IsAssignableFrom(runtimeType)) return DeclarationKind.Transaction;
        if (typeof(Event).IsAssignableFrom(runtimeType)) return DeclarationKind.Event;
        return DeclarationKind.Concept;
    }

    private static string? ComputeSuperTypeFqn(Type runtimeType)
    {
        var type = runtimeType.BaseType;
        while (type is not null)
        {
            var attr = type.GetCustomAttribute<TypeAttribute>(inherit: false);
            if (attr is not null && typeof(Concept).IsAssignableFrom(type))
            {
                return attr.ToType().ToString();
            }

            type = type.BaseType;
        }

        return null;
    }

    private static PropertyInfo? FindIdentifierProperty(Type type, bool declaredOnly)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        if (declaredOnly)
        {
            flags |= BindingFlags.DeclaredOnly;
        }

        var identifierProperty = type.GetProperties(flags)
            .FirstOrDefault(property => property.GetCustomAttribute<IdentifierAttribute>(inherit: true) is not null);
        if (identifierProperty is not null || declaredOnly)
        {
            return identifierProperty;
        }

        return type.BaseType is null ? null : FindIdentifierProperty(type.BaseType, declaredOnly: false);
    }

    private static string ComputeTypeName(Type unwrappedType)
    {
        if (PrimitiveTypeNames.TryGetValue(unwrappedType, out var name))
        {
            return name;
        }

        var typeAttribute = unwrappedType.GetCustomAttribute<TypeAttribute>(inherit: false);
        return typeAttribute?.Name ?? unwrappedType.Name;
    }

    private static bool ComputeIsArray(Type propertyType)
        => propertyType != typeof(string)
           && propertyType != typeof(byte[])
           && typeof(IEnumerable).IsAssignableFrom(propertyType);

    private static bool ComputeIsOptional(PropertyInfo pi)
    {
        if (Nullable.GetUnderlyingType(pi.PropertyType) is not null)
        {
            return true;
        }

        if (!pi.PropertyType.IsValueType)
        {
            var nullability = NullabilityContext.Create(pi);
            return nullability.WriteState == NullabilityState.Nullable;
        }

        return false;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(static type => type is not null)!;
        }
    }

    private static bool IsConcertoClassType(Type type)
        => type.IsClass && typeof(Concept).IsAssignableFrom(type) && GetTypeAttribute(type) is not null;

    private static TypeAttribute? GetTypeAttribute(MemberInfo member)
        => member.GetCustomAttribute<TypeAttribute>(inherit: false);
}
