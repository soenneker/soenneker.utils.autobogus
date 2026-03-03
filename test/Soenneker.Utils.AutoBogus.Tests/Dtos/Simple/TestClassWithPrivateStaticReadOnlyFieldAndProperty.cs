namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithPrivateStaticReadOnlyFieldAndProperty
{
    private static readonly string DefaultValue = "test";

    public string Property { get; set; }
}
