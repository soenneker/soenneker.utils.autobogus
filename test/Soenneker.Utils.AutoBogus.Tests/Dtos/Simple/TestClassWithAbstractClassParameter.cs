namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithAbstractClassParameter
{
    public TestAbstractClassWithCtor TestClass { get; set; }

    public TestClassWithAbstractClassParameter(TestAbstractClassWithCtor testClass)
    {
        TestClass = testClass;
    }
}