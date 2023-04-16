using System.IO;
using System.Reflection;
using DIGraph;

var commandArgs = Environment.GetCommandLineArgs().ToList();
var currentPath = Path.GetDirectoryName(commandArgs[0]) ?? "";
var targetPath = commandArgs.Count > 1 ? Path.GetFullPath(Path.Combine(currentPath, commandArgs[1])) : currentPath;
var namespacePrefix = commandArgs.Count > 2 ? commandArgs[2] : "";
Console.WriteLine($"Looking in {targetPath}");
var dllFiles = Directory.GetFiles(targetPath, $"{namespacePrefix}*.dll", SearchOption.AllDirectories).ToList();

Console.WriteLine("Assemblies Found:");
dllFiles.ForEach(Console.WriteLine);

var allInjectedDependencies = dllFiles
    .SelectMany(dllFile => Assembly
        .LoadFile(dllFile)
        .FindInjectedDependencyNames(namespacePrefix))
    .ToList();
var interfaces = allInjectedDependencies
    .GroupBy(dep => dep.DependencyName)
    .Select(group => group.First())
    .ToList();

Console.WriteLine("Injected Dependencies Found:");
Console.WriteLine("```mermaid");
Console.WriteLine("flowchart TD");
allInjectedDependencies
    .Select(dep => $"  {dep.ClassName} --> {dep.DependencyName}")
    .Distinct()
    .ToList()
    .ForEach(Console.WriteLine);
interfaces
    .Select(api => @$"
    subgraph {api.DependencyName}
        {string.Join("\n        ", api.DependencySubTypes ?? new List<string>())}
    end
    ")
    .ToList()
    .ForEach(Console.WriteLine);
Console.WriteLine("```");
