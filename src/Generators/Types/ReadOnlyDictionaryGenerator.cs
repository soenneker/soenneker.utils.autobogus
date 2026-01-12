using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class ReadOnlyDictionaryGenerator<TKey, TValue> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        Type generateType = context.CachedType.Type!;

        if (context.CachedType.IsInterface)
            generateType = typeof(Dictionary<TKey, TValue>);

        // Create an instance of a dictionary (public and non-public)
        IDictionary<TKey, TValue> items;

        if (context.CachedType.IsInterface || context.CachedType.Type.Name == "Dictionary`2")
        {
            items = new Dictionary<TKey, TValue>();
        }
        else
        {
            try
            {
                items = (IDictionary<TKey, TValue>) context.CachedType.CreateInstance();

                if (items == null)
                    items = new Dictionary<TKey, TValue>();
            }
            catch //(Exception e)
            {
                items = new Dictionary<TKey, TValue>();
            }
        }

        // Get a list of keys
        List<TKey> keys = context.GenerateUniqueMany<TKey>();

        int length = keys.Count;

        for (var i = 0; i < length; i++)
        {
            TKey key = keys[i];

            // Get a matching value for the current key and add to the dictionary
            var value = context.Generate<TValue>();

            if (value != null)
            {
                items.Add(key, value);
            }
        }

        // Avoid Activator.CreateInstance(params object[]) allocation where possible.
        if (generateType == typeof(Dictionary<TKey, TValue>))
        {
            return items is Dictionary<TKey, TValue> d ? d : new Dictionary<TKey, TValue>(items);
        }

        if (generateType == typeof(ReadOnlyDictionary<TKey, TValue>))
        {
            return new ReadOnlyDictionary<TKey, TValue>(items);
        }

        var cachedGenType = context.CacheService.Cache.GetCachedType(generateType);

        // Prefer IDictionary<TKey,TValue> ctor; fall back to reflection if needed.
        var ctor = cachedGenType.GetCachedConstructor(typeof(IDictionary<TKey, TValue>));
        if (ctor != null)
            return ctor.Invoke(items);

        return Activator.CreateInstance(generateType, items);
    }
}