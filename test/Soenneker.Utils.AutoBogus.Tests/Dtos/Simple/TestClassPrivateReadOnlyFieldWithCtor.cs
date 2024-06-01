namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassPrivateReadOnlyFieldWithCtor
{
    private readonly string _key;

    public TestClassPrivateReadOnlyFieldWithCtor(string key)
    {
        _key = key;
    }

    public string GetKey()
    {
        return _key;
    }
}