using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassIDictionaryPropertyWrappedWithReadOnly
{
    // Both fields are readonly but the dictionaries can be modified.
    private readonly Dictionary<string, string> _dictionary = new();
    private readonly Dictionary<string, string> _dictionaryToWrap = new();

    // The returned dictionary is not wrapped and the Add method can be called.
    public IDictionary<string, string> Dictionary => _dictionary;

    // The returned dictionary is wrapped with ReadOnlyDictionary which throws an exception when the Add method is called.
    public IDictionary<string, string> WrappedDictionary => _dictionaryToWrap.AsReadOnly();
}