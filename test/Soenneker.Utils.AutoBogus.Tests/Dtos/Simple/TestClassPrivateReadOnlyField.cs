namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassPrivateReadOnlyField
{
    private readonly string _key;

    public TestClassPrivateReadOnlyField()
    {
    }

    public string GetKey()
    {
        return _key;
    }
}