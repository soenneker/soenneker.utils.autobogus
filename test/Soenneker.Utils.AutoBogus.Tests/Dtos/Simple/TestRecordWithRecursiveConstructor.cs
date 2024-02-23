using System.Dynamic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

internal record TestRecordWithRecursiveConstructor
{
    public string? Name { get; set; }
    public int Age { get; set; }

    public TestRecordWithRecursiveConstructor Child { get; set; }

    public TestRecordWithRecursiveConstructor(string? name, int age, TestRecordWithRecursiveConstructor? child)
    {
        Name = name;
        Age = age;
        Child = child;
    }
}