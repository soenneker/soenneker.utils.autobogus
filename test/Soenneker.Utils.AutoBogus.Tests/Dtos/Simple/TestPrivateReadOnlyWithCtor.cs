namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestPrivateReadOnlyWithCtor
{
    private readonly string _key;

    public TestPrivateReadOnlyWithCtor(string key)
    {
        _key = key;
    }

    public string GetKey()
    {
        return _key;
    }
}