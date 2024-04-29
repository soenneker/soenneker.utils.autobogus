namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public abstract class TestAbstractClassWithCtor
{
    public string Name { get; set; }

    public TestAbstractClassWithCtor(string name)
    {
        Name = name;
    }

}