using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

internal class TestClassWithRecursiveEnumerableConstructor
{
    public string? Name { get; set; }
    public int Age { get; set; }

    public IEnumerable<TestClassWithRecursiveEnumerableConstructor> Child { get; set; }

    public TestClassWithRecursiveEnumerableConstructor(string? name, int age, IEnumerable<TestClassWithRecursiveEnumerableConstructor> child)
    {
        Name = name;
        Age = age;
        Child = child;
    }
}