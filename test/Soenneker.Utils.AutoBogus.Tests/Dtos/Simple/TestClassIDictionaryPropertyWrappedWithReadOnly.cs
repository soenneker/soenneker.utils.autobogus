using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassIDictionaryPropertyWrappedWithReadOnly
{
    private readonly Dictionary<string, string> _dictionary = new();

    public IDictionary<string, string> Dictionary => _dictionary.AsReadOnly();
}