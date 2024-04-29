namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithDerivedClassParameter
{
    public TestDerivedClass TestClass { get; set; }

    public TestClassWithDerivedClassParameter(TestDerivedClass testClass)
    {
        TestClass = testClass;
    }
}