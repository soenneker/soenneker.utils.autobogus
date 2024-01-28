using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class DictionaryGenerator<TKey, TValue>
    : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        // Create an instance of a dictionary (public and non-public)
        IDictionary<TKey, TValue> items;

        if (context.CachedType.IsInterface || context.GenerateType.Name == "Dictionary`2")
        {
            items = new Dictionary<TKey, TValue>();
        }
        else
        {
            try
            {
                items = (IDictionary<TKey, TValue>)Activator.CreateInstance(context.GenerateType, true);
                //items = context.CachedType.CreateInstance<IDictionary<TKey, TValue>>(); //(IDictionary<TKey, TValue>)Activator.CreateInstance(context.GenerateType, true);
            }
            catch
            {
                items = new Dictionary<TKey, TValue>();
            }
        }

        // Get a list of keys
        List<TKey> keys = context.GenerateUniqueMany<TKey>();

        int length = keys.Count;

        for (int i = 0; i < length; i++)
        {
            TKey key = keys[i];

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