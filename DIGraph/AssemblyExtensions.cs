using System.Reflection;
using DIGraph.Models;

namespace DIGraph;

public static class AssemblyExtentions
{
    public static List<InjectedDependency> FindInjectedDependencyNames(this Assembly assembly, string withNameSpacingStartingWith = "")
    {
        var allTypes = assembly.GetTypes();

        return allTypes
            .Where(type => type.IsClass && (type.Namespace ?? "").StartsWith(withNameSpacingStartingWith))
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
