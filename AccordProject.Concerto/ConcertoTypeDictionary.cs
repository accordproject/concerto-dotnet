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

using System.Reflection;
using AccordProject.Concerto;

/// <summary>
/// This singleton class provides a dictionary that maps Concerto types to .NET class types.
/// </summary>
public class ConcertoTypeDictionary
{
    private static readonly Lazy<ConcertoTypeDictionary> lazy = new Lazy<ConcertoTypeDictionary>(() => new ConcertoTypeDictionary());
    
    public static ConcertoTypeDictionary Instance { get { return lazy.Value; } }

    private Dictionary<ConcertoType, Type> entries = new Dictionary<ConcertoType, Type>();

    private ConcertoTypeDictionary()
    {
        var currentDomain = AppDomain.CurrentDomain;
        // Subscribe to event that informs us of any new assemblies.
        currentDomain.AssemblyLoad += AssemblyLoaded;
        LoadTypesFromAppDomain(currentDomain);
    }

    ~ConcertoTypeDictionary()
    {
        var currentDomain = AppDomain.CurrentDomain;
        // Unsubscribe from event that informs us of any new assemblies.
        currentDomain.AssemblyLoad -= AssemblyLoaded;
    }

    public Type? ResolveType(string fqn)
    {
        var type = ConcertoUtils.ParseType(fqn);
        return ResolveType(type);
    }

    public Type? ResolveType(ConcertoType type)
    {
        var exists = entries.TryGetValue(type, out Type? value);
        if (!exists)
        {
            return null;
        }
        return value;
    }

    private void AssemblyLoaded(object? sender, AssemblyLoadEventArgs args)
    {
        var assembly = args.LoadedAssembly;
        LoadTypesFromAssembly(assembly);
    }

    private void LoadTypesFromAppDomain(AppDomain appDomain)
    {
        var assemblies = appDomain.GetAssemblies();
        LoadTypesFromAssemblies(assemblies);
    }

    private void LoadTypesFromAssemblies(Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            LoadTypesFromAssembly(assembly);
        }
    }

    private void LoadTypesFromAssembly(Assembly assembly)
    {
        var types = GetLoadableTypes(assembly).Where(
            type =>
                type.IsClass
                && typeof(Concept).IsAssignableFrom(type)
                && type.GetCustomAttribute<TypeAttribute>() != null);
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<TypeAttribute>();
            var key = attribute!.ToType();
            entries.Add(key, type);
        }
    }

    public IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null)!;
        }
    }
}   