namespace DIGraph.Models;

public record InjectedDependency
{
    public string? ClassName { get; set; }
    public string? DependencyName { get; set; }
    public List<string>? DependencySubTypes { get; set; }
}