using System.Dynamic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

internal class TestClassWithRecursiveConstructor
{
    public string? Name { get; set; }
    public int Age { get; set; }

    public TestClassWithRecursiveConstructor Child { get; set; }

    public TestClassWithRecursiveConstructor(string? name, int age, TestClassWithRecursiveConstructor? child)
    {
        Name = name;
        Age = age;
        Child = child;
    }
}