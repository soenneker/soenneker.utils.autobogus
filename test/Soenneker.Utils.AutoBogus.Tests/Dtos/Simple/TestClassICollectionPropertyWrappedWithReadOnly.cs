using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassICollectionPropertyWrappedWithReadOnly
{
    // Both fields are readonly but the collections can be modified.
    private readonly List<int> _collection = new();
    private readonly List<int> _collectionToWrap = new();

    // The returned collection is not wrapped and the Add method can be called.
    public ICollection<int> Collection => _collection;

    // The returned collection is wrapped with ReadOnlyCollection which throws an exception when the Add method is called.
    public ICollection<int> WrappedCollection => _collectionToWrap.AsReadOnly();
}