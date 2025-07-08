namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithArrayStringProperty
{
    public string[] Values { get; } = new string[2]; // Fixed-size array, no setter
}