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
using System.Collections.ObjectModel;
using System.Reflection;

namespace AccordProject.Concerto;

public sealed class ModelManager
{
    private readonly Dictionary<string, ClassDeclaration> classDeclarations = new(StringComparer.Ordinal);
    private readonly Dictionary<string, EnumDeclaration> enumDeclarations = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ModelFile> modelFiles = new(StringComparer.Ordinal);
    private readonly HashSet<Assembly> registeredAssemblies = new();
    private readonly object syncRoot = new();
    private readonly Introspector introspector;

    public ModelManager()
        : this(AppDomain.CurrentDomain.GetAssemblies())
    {
    }

    public ModelManager(IEnumerable<Assembly> assemblies)
    {
        introspector = new Introspector(this);
        foreach (var assembly in assemblies)
        {
            RegisterAssembly(assembly);
        }
    }

    public Introspector GetIntrospector()
    {
        return introspector;
    }

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

        foreach (var type in loadableTypes.Where(IsConcertoClassType))
        {
            RegisterClassDeclaration(type);

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
    }

    public void RegisterType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type.IsEnum)
        {
            var typeAttribute = GetTypeAttribute(type);
            if (typeAttribute is null)
            {
                throw new InvalidOperationException($"Cannot register enum '{type.FullName}' without a {nameof(TypeAttribute)}.");
            }

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
    {
        return modelFiles.Values
            .OrderBy(modelFile => modelFile.GetNamespace(), StringComparer.Ordinal)
            .ToArray();
    }

    public ModelFile? GetModelFile(string @namespace)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(@namespace);
        modelFiles.TryGetValue(@namespace, out var modelFile);
        return modelFile;
    }

    public IEnumerable<string> GetNamespaces()
    {
        return modelFiles.Keys.OrderBy(static item => item, StringComparer.Ordinal).ToArray();
    }

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
    {
        return classDeclarations.Values
            .OrderBy(declaration => declaration.FullyQualifiedName, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyCollection<EnumDeclaration> GetEnumDeclarations()
    {
        return enumDeclarations.Values
            .OrderBy(declaration => declaration.FullyQualifiedName, StringComparer.Ordinal)
            .ToArray();
    }

    internal IEnumerable<ClassDeclaration> FindAssignableClassDeclarations(ClassDeclaration declaration)
    {
        return classDeclarations.Values
            .Where(candidate => declaration.RuntimeType.IsAssignableFrom(candidate.RuntimeType))
            .OrderBy(candidate => candidate.FullyQualifiedName, StringComparer.Ordinal)
            .ToArray();
    }

    internal IEnumerable<ClassDeclaration> FindDirectSubclasses(ClassDeclaration declaration)
    {
        return classDeclarations.Values
            .Where(candidate => candidate.RuntimeType.BaseType == declaration.RuntimeType)
            .OrderBy(candidate => candidate.FullyQualifiedName, StringComparer.Ordinal)
            .ToArray();
    }

    internal EnumDeclaration? TryGetEnum(Type runtimeType)
    {
        return enumDeclarations.Values.FirstOrDefault(declaration => declaration.RuntimeType == runtimeType);
    }

    internal IEnumerable<Type> GetLoadableTypes(Assembly assembly)
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
    {
        return type.IsClass
            && typeof(Concept).IsAssignableFrom(type)
            && GetTypeAttribute(type) is not null;
    }

    private static TypeAttribute? GetTypeAttribute(MemberInfo member)
    {
        return member.GetCustomAttribute<TypeAttribute>(inherit: false);
    }

    private void RegisterClassDeclaration(Type runtimeType)
    {
        var attribute = GetTypeAttribute(runtimeType)
            ?? throw new InvalidOperationException($"Type '{runtimeType.FullName}' is missing a {nameof(TypeAttribute)}.");
        var declaration = new ClassDeclaration(this, runtimeType, attribute);
        AddDeclaration(declaration);
    }

    private void RegisterEnumDeclaration(Type runtimeType, ConcertoNamespace concertoNamespace)
    {
        var declaration = new EnumDeclaration(this, runtimeType, concertoNamespace.Namespace, concertoNamespace.Version, runtimeType.Name);
        AddDeclaration(declaration);
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

    internal static IEnumerable<PropertyInfo> GetDeclaredModelProperties(Type runtimeType)
    {
        return runtimeType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static property => property.GetIndexParameters().Length == 0)
            .Where(static property => GetSerializedPropertyName(property) != "$class")
            .OrderBy(static property => property.MetadataToken);
    }

    internal static string GetSerializedPropertyName(PropertyInfo property)
    {
        var jsonProperty = property.GetCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>(inherit: true);
        if (!string.IsNullOrWhiteSpace(jsonProperty?.PropertyName))
        {
            return jsonProperty!.PropertyName!;
        }

        return property.Name;
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
}

public sealed class Introspector
{
    private readonly ModelManager modelManager;

    public Introspector(ModelManager modelManager)
    {
        this.modelManager = modelManager ?? throw new ArgumentNullException(nameof(modelManager));
    }

    public IReadOnlyCollection<ClassDeclaration> GetClassDeclarations()
    {
        return modelManager.GetClassDeclarations();
    }

    public ClassDeclaration GetClassDeclaration(string fullyQualifiedTypeName)
    {
        return modelManager.GetType(fullyQualifiedTypeName)
            ?? throw new KeyNotFoundException($"Unknown class declaration '{fullyQualifiedTypeName}'.");
    }
}

public sealed class ModelFile
{
    private readonly List<Declaration> declarations = new();
    private readonly ReadOnlyCollection<Declaration> readonlyDeclarations;

    internal ModelFile(ModelManager modelManager, string @namespace, string? version)
    {
        ModelManager = modelManager;
        Namespace = @namespace;
        Version = version;
        readonlyDeclarations = declarations.AsReadOnly();
    }

    public ModelManager ModelManager { get; }

    public string Namespace { get; }

    public string? Version { get; }

    public string GetNamespace()
    {
        return Namespace;
    }

    public string? GetVersion()
    {
        return Version;
    }

    public IReadOnlyCollection<Declaration> GetAllDeclarations()
    {
        return readonlyDeclarations;
    }

    public IReadOnlyCollection<ClassDeclaration> GetClassDeclarations()
    {
        return declarations.OfType<ClassDeclaration>().ToArray();
    }

    public IReadOnlyCollection<ClassDeclaration> GetAssetDeclarations()
    {
        return declarations.OfType<ClassDeclaration>().Where(static declaration => declaration.IsAsset()).ToArray();
    }

    public IReadOnlyCollection<ClassDeclaration> GetParticipantDeclarations()
    {
        return declarations.OfType<ClassDeclaration>().Where(static declaration => declaration.IsParticipant()).ToArray();
    }

    public IReadOnlyCollection<ClassDeclaration> GetTransactionDeclarations()
    {
        return declarations.OfType<ClassDeclaration>().Where(static declaration => declaration.IsTransaction()).ToArray();
    }

    public IReadOnlyCollection<ClassDeclaration> GetEventDeclarations()
    {
        return declarations.OfType<ClassDeclaration>().Where(static declaration => declaration.IsEvent()).ToArray();
    }

    public IReadOnlyCollection<ClassDeclaration> GetConceptDeclarations()
    {
        return declarations.OfType<ClassDeclaration>().Where(static declaration => declaration.IsConcept()).ToArray();
    }

    public IReadOnlyCollection<EnumDeclaration> GetEnumDeclarations()
    {
        return declarations.OfType<EnumDeclaration>().ToArray();
    }

    internal void AddDeclaration(Declaration declaration)
    {
        if (declarations.Any(existing => existing.FullyQualifiedName == declaration.FullyQualifiedName))
        {
            declarations.RemoveAll(existing => existing.FullyQualifiedName == declaration.FullyQualifiedName);
        }

        declarations.Add(declaration);
        declarations.Sort(static (left, right) => StringComparer.Ordinal.Compare(left.FullyQualifiedName, right.FullyQualifiedName));
    }
}

public abstract class DecoratedElement
{
    private readonly ReadOnlyCollection<DecoratorInfo> decorators;

    protected DecoratedElement(IEnumerable<DecoratorInfo> decorators)
    {
        this.decorators = decorators.ToArray().AsReadOnly();
    }

    public IReadOnlyCollection<DecoratorInfo> GetDecorators()
    {
        return decorators;
    }

    public DecoratorInfo? GetDecorator(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return decorators.FirstOrDefault(decorator => string.Equals(decorator.Name, name, StringComparison.Ordinal));
    }
}

public abstract class Declaration : DecoratedElement
{
    protected Declaration(ModelManager modelManager, string @namespace, string? version, string name, IEnumerable<DecoratorInfo> decorators)
        : base(decorators)
    {
        ModelManager = modelManager;
        Namespace = @namespace;
        Version = version;
        Name = name;
    }

    public ModelManager ModelManager { get; }

    public string Namespace { get; }

    public string? Version { get; }

    public string Name { get; }

    public string FullyQualifiedName => new ConcertoType { Namespace = Namespace, Version = Version, Name = Name }.ToString();

    public ModelFile GetModelFile()
    {
        return modelFile ?? throw new InvalidOperationException($"Model file for '{FullyQualifiedName}' has not been attached.");
    }

    internal void AttachModelFile(ModelFile file)
    {
        modelFile = file;
    }

    private ModelFile? modelFile;
}

public sealed class ClassDeclaration : Declaration
{
    private readonly ReadOnlyCollection<Property> ownProperties;
    private readonly TypeAttribute typeAttribute;

    internal ClassDeclaration(ModelManager modelManager, Type runtimeType, TypeAttribute typeAttribute)
        : base(modelManager, typeAttribute.Namespace, typeAttribute.Version, typeAttribute.Name, DecoratorInfo.FromMember(runtimeType))
    {
        RuntimeType = runtimeType;
        this.typeAttribute = typeAttribute;
        ownProperties = ModelManager.GetDeclaredModelProperties(runtimeType)
            .Select(property => new Property(this, property))
            .ToArray()
            .AsReadOnly();
    }

    internal Type RuntimeType { get; }

    public bool IsAbstract()
    {
        return RuntimeType.IsAbstract;
    }

    public bool IsExplicitlyIdentified()
    {
        return FindIdentifierProperty(RuntimeType, declaredOnly: true) is not null;
    }

    public bool IsIdentified()
    {
        return GetIdentifierFieldName() is not null;
    }

    public string? GetIdentifierFieldName()
    {
        var identifierProperty = FindIdentifierProperty(RuntimeType, declaredOnly: false);
        return identifierProperty is null ? null : ModelManager.GetSerializedPropertyName(identifierProperty);
    }

    public Property? GetOwnProperty(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return ownProperties.FirstOrDefault(property => string.Equals(property.Name, name, StringComparison.Ordinal));
    }

    public IReadOnlyCollection<Property> GetOwnProperties()
    {
        return ownProperties;
    }

    public string? GetSuperType()
    {
        return GetSuperTypeDeclaration()?.FullyQualifiedName;
    }

    public ClassDeclaration? GetSuperTypeDeclaration()
    {
        var type = RuntimeType.BaseType;
        while (type is not null)
        {
            var typeAttribute = type.GetCustomAttribute<TypeAttribute>(inherit: false);
            if (typeAttribute is not null && typeof(Concept).IsAssignableFrom(type))
            {
                return ModelManager.GetType(typeAttribute.ToType().ToString());
            }

            type = type.BaseType;
        }

        return null;
    }

    public IReadOnlyCollection<ClassDeclaration> GetAssignableClassDeclarations()
    {
        return ModelManager.FindAssignableClassDeclarations(this).ToArray();
    }

    public IReadOnlyCollection<ClassDeclaration> GetDirectSubclasses()
    {
        return ModelManager.FindDirectSubclasses(this).ToArray();
    }

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

    public bool IsAsset()
    {
        return typeof(Asset).IsAssignableFrom(RuntimeType);
    }

    public bool IsParticipant()
    {
        return typeof(Participant).IsAssignableFrom(RuntimeType);
    }

    public bool IsTransaction()
    {
        return typeof(Transaction).IsAssignableFrom(RuntimeType);
    }

    public bool IsEvent()
    {
        return typeof(Event).IsAssignableFrom(RuntimeType);
    }

    public bool IsConcept()
    {
        return typeof(Concept).IsAssignableFrom(RuntimeType);
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
}

public sealed class EnumDeclaration : Declaration
{
    internal EnumDeclaration(ModelManager modelManager, Type runtimeType, string @namespace, string? version, string name)
        : base(modelManager, @namespace, version, name, DecoratorInfo.FromMember(runtimeType))
    {
        RuntimeType = runtimeType;
    }

    internal Type RuntimeType { get; }

    public IReadOnlyCollection<string> GetValues()
    {
        return Enum.GetNames(RuntimeType);
    }
}

public sealed class Property : DecoratedElement
{
    private static readonly NullabilityInfoContext NullabilityContext = new();
    private static readonly Dictionary<Type, string> PrimitiveTypeNames = new()
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

    internal Property(ClassDeclaration parent, PropertyInfo propertyInfo)
        : base(DecoratorInfo.FromMember(propertyInfo))
    {
        Parent = parent;
        PropertyInfo = propertyInfo;
        Name = ModelManager.GetSerializedPropertyName(propertyInfo);
    }

    public ClassDeclaration Parent { get; }

    public string Name { get; }

    internal PropertyInfo PropertyInfo { get; }

    public string TypeName => ResolveTypeName();

    public ClassDeclaration GetParent()
    {
        return Parent;
    }

    public bool IsOptional()
    {
        if (Nullable.GetUnderlyingType(PropertyInfo.PropertyType) is not null)
        {
            return true;
        }

        if (!PropertyInfo.PropertyType.IsValueType)
        {
            var nullability = NullabilityContext.Create(PropertyInfo);
            return nullability.WriteState == NullabilityState.Nullable;
        }

        return false;
    }

    public bool IsArray()
    {
        return PropertyInfo.PropertyType != typeof(string)
            && PropertyInfo.PropertyType != typeof(byte[])
            && typeof(IEnumerable).IsAssignableFrom(PropertyInfo.PropertyType);
    }

    public bool IsTypeEnum()
    {
        return ModelManager.UnwrapPropertyType(PropertyInfo.PropertyType).IsEnum;
    }

    public bool IsPrimitive()
    {
        return PrimitiveTypeNames.ContainsKey(ModelManager.UnwrapPropertyType(PropertyInfo.PropertyType));
    }

    public bool IsRelationship()
    {
        return PropertyInfo.GetCustomAttribute<RelationshipAttribute>(inherit: true) is not null;
    }

    public string GetFullyQualifiedTypeName()
    {
        var propertyType = ModelManager.UnwrapPropertyType(PropertyInfo.PropertyType);
        if (PrimitiveTypeNames.TryGetValue(propertyType, out var primitiveTypeName))
        {
            return primitiveTypeName;
        }

        if (propertyType.IsEnum)
        {
            return Parent.ModelManager.TryGetEnum(propertyType)?.FullyQualifiedName
                ?? new ConcertoType { Namespace = Parent.Namespace, Version = Parent.Version, Name = propertyType.Name }.ToString();
        }

        var typeAttribute = propertyType.GetCustomAttribute<TypeAttribute>(inherit: false);
        if (typeAttribute is not null)
        {
            return typeAttribute.ToType().ToString();
        }

        return propertyType.FullName ?? propertyType.Name;
    }

    public string GetFullyQualifiedName()
    {
        return $"{Parent.FullyQualifiedName}.{Name}";
    }

    public string GetNamespace()
    {
        return Parent.Namespace;
    }

    private string ResolveTypeName()
    {
        var propertyType = ModelManager.UnwrapPropertyType(PropertyInfo.PropertyType);
        if (PrimitiveTypeNames.TryGetValue(propertyType, out var primitiveTypeName))
        {
            return primitiveTypeName;
        }

        var typeAttribute = propertyType.GetCustomAttribute<TypeAttribute>(inherit: false);
        if (typeAttribute is not null)
        {
            return typeAttribute.Name;
        }

        return propertyType.Name;
    }
}

public sealed class DecoratorInfo
{
    public DecoratorInfo(string name, IEnumerable<object?> arguments)
    {
        Name = name;
        Arguments = arguments.ToArray().AsReadOnly();
    }

    public string Name { get; }

    public IReadOnlyList<object?> Arguments { get; }

    internal static IReadOnlyCollection<DecoratorInfo> FromMember(MemberInfo member)
    {
        return member.GetCustomAttributes<DecoratorAttribute>(inherit: true)
            .Select(attribute => new DecoratorInfo(attribute.Name, attribute.Arguments))
            .ToArray();
    }
}