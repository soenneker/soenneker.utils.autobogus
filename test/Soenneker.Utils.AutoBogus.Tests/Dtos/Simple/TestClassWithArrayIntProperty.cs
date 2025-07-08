namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithArrayIntProperty
{
    public int[] Values { get; } = new int[2]; // Fixed-size array, no setter
}