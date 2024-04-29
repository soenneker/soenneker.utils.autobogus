namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestDerivedClass : TestAbstractClassWithCtor
{
    public TestDerivedClass(string name) : base(name)
    {
    }
}