using System.Reflection;
using DIGraph.Models;
using FluentAssertions;

namespace DIGraph.Tests;

public interface IDoSomething { };
public interface IDoSomethingElse { };

public class TestComponent1
{
    public TestComponent1(IDoSomething something, IDoSomethingElse somethingElse) { }
}

public class TestComponent2
{
    public TestComponent2(IDoSomething something) { }
}

public class AssemblyExtensionsTests
{
    [Fact]
    public void FindInjectedDependencies()
    {
        Assembly
            .GetExecutingAssembly()
            .FindInjectedDependencyNames(withNameSpacingStartingWith: "DIGraph.")
            .Should()
            .BeEquivalentTo(new List<InjectedDependency>
            {
                new InjectedDependency
                {
                    ClassName = "DIGraph.Tests.TestComponent1",
                    DependencyName = "DIGraph.Tests.IDoSomething"
                },
                new InjectedDependency
                {
                    ClassName = "DIGraph.Tests.TestComponent1",
                    DependencyName = "DIGraph.Tests.IDoSomethingElse"
                },
                new InjectedDependency
                {
                    ClassName = "DIGraph.Tests.TestComponent2",
                    DependencyName = "DIGraph.Tests.IDoSomething"
                }
            });
    }
}