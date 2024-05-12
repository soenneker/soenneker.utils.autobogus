using System.Collections.Immutable;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos;

public class ImmutableListDto
{
    public ImmutableList<int> List { get; set; }

    public IImmutableList<int> IList { get; set; }
}