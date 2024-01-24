using System.Collections;
using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class DictionaryDerivedClass : IDictionary<string, string>
{
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<string, string> item)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(KeyValuePair<string, string> item)
    {
        throw new System.NotImplementedException();
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(KeyValuePair<string, string> item)
    {
        throw new System.NotImplementedException();
    }

    public int Count { get; }
    public bool IsReadOnly { get; }
    public void Add(string key, string value)
    {
        throw new System.NotImplementedException();
    }

    public bool ContainsKey(string key)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(string key)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetValue(string key, out string value)
    {
        throw new System.NotImplementedException();
    }

    public string this[string key]
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public ICollection<string> Keys { get; }
    public ICollection<string> Values { get; }
}