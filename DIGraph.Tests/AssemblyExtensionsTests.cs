using System.Reflection;
using DIGraph.Models;
using FluentAssertions;

namespace DIGraph.Tests;

public interface IDoSomething { };
public interface IDoSomethingElse { };

public class DoSomethingStupid : IDoSomething { };

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
                    DependencyName = "DIGraph.Tests.IDoSomething",
                    DependencySubTypes = new List<string>
                    {
                        "DIGraph.Tests.DoSomethingStupid"
                    }
                },
                new InjectedDependency
                {
                    ClassName = "DIGraph.Tests.TestComponent1",
                    DependencyName = "DIGraph.Tests.IDoSomethingElse",
                    DependencySubTypes = new List<string>()
                },
                new InjectedDependency
                {
                    ClassName = "DIGraph.Tests.TestComponent2",
                    DependencyName = "DIGraph.Tests.IDoSomething",
                    DependencySubTypes = new List<string>
                    {
                        "DIGraph.Tests.DoSomethingStupid"
                    }
                }
            });
    }
}