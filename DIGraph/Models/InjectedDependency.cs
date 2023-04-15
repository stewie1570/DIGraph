namespace DIGraph.Models;

public record InjectedDependency
{
    public string? ClassName { get; set; }
    public string? DependencyName { get; set; }
}