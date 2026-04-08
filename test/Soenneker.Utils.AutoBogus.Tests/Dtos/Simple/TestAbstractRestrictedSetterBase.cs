namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public abstract class TestAbstractRestrictedSetterBase
{
    public string? Title { get; private set; }

    public int? Sequence { get; protected set; }

    public int? Magnitude { get; private protected set; }
}
