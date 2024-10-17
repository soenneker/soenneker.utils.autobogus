using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassWithCollectionBackedByReadOnlyCollection<T>
{
    public IReadOnlyCollection<T> PublicList { get; private set; }

    protected ICollection<T> InternalList => (ICollection<T>)PublicList;
}