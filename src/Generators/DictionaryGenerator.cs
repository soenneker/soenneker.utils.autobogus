using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class DictionaryGenerator<TKey, TValue>
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        // Create an instance of a dictionary (public and non-public)
        IDictionary<TKey, TValue> items;
        try
        {
            items = (IDictionary<TKey, TValue>)Activator.CreateInstance(context.GenerateType, true);
        }
        catch
        {
            items = new Dictionary<TKey, TValue>();
        }
      
        // Get a list of keys
        List<TKey> keys = context.GenerateUniqueMany<TKey>();

        foreach (TKey? key in keys)
        {
            // Get a matching value for the current key and add to the dictionary
            var value = context.Generate<TValue>();

            if (value != null)
            {
                items.Add(key, value);
            }
        }

        return items;
    }
}