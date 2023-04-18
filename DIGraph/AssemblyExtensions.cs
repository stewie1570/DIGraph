using System.Reflection;
using DIGraph.Models;

namespace DIGraph;

public static class AssemblyExtentions
{
    public static List<InjectedDependency> FindInjectedDependencyNames(this Assembly assembly, string withNameSpacingStartingWith = "")
    {
        return new List<Assembly> { assembly }.FindInjectedDependencyNames(withNameSpacingStartingWith);
    }

    public static List<InjectedDependency> FindInjectedDependencyNames(this IEnumerable<Assembly?> assemblies, string withNameSpacingStartingWith = "")
    {
        var allTypes = assemblies
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly?.DefinedTypes ?? new List<TypeInfo>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ASSEMBLY: {assembly?.FullName} could not load. ({ex.GetType()}\n{ex.Message}\n{ex.StackTrace})");
                    return new List<TypeInfo>();
                }
            })
            .ToList();

        return allTypes
            .Where(type => (type.Namespace ?? "").StartsWith(withNameSpacingStartingWith) && type.IsClass)
            .SelectMany(theClass => theClass.GetConstructors().Select(constructor => new
            {
                Constructor = constructor,
                TheClass = theClass
            }))
            .SelectMany(dep => dep.Constructor.GetParameters().Select(param => new InjectedDependency
            {
                ClassName = dep.TheClass.GetDisplayName(),
                DependencyName = param.ParameterType.GetDisplayName(),
                DependencySubTypes = allTypes
                    .Where(type => type.IsClass && type.IsAssignableTo(param.ParameterType))
                    .Select(type => type.GetDisplayName())
                    .ToList()
            }))
            .ToList();
    }

    #region Helpers

    private static string GetDisplayName(this Type type)
    {
        return $"{type.Namespace}.{type.Name}";
    }

    #endregion
}
