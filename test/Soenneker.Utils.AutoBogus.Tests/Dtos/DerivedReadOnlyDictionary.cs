using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos;

public class DerivedReadOnlyDictionary : ReadOnlyDictionary<string, object>
{
    public DerivedReadOnlyDictionary(IDictionary<string, object> dictionary) : base(dictionary)
    {
    }
}