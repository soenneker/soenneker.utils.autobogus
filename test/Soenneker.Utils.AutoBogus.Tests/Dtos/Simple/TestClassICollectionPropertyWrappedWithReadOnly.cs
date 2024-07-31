using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassICollectionPropertyWrappedWithReadOnly
{
    private readonly List<int> _collection = new();

    public ICollection<int> Collection => _collection.AsReadOnly();
}