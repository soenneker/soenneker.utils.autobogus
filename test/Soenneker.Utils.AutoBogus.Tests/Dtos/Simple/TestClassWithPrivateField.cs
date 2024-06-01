namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithPrivateField
{
    private string _value;

    public string GetValue()
    {
        return _value;
    }
}