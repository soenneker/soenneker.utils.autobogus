using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public sealed class OverrideClass
{
    public OverrideClass()
    {
        Id = new OverrideId();
    }

    public OverrideId Id { get; }
    public string Name { get; set; }
    public IEnumerable<int> Amounts { get; set; }
}