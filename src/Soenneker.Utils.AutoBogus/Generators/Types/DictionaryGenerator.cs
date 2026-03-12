using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class DictionaryGenerator<TKey, TValue> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        int target = context.Config.RepeatCount > 0 ? context.Config.RepeatCount : 0;

        if (target == 0)
            return new Dictionary<TKey, TValue>(0);

        // Create instance (prefer exact Dictionary<,> for fast path + pre-sizing)
        IDictionary<TKey, TValue> items;
        CachedType? ct = context.CachedType;

        bool isExactBclDict = ct.Type.IsConstructedGenericType &&
                              ct.Type.GetGenericTypeDefinition() == typeof(Dictionary<,>);

        if (!ct.IsInterface && !isExactBclDict)
        {
            try
            {
                items = (IDictionary<TKey, TValue>)ct.CreateInstance();
            }
            catch
            {
                items = new Dictionary<TKey, TValue>(target);
            }
        }
        else
        {
            items = new Dictionary<TKey, TValue>(target);
        }

        // Cache delegates to help the JIT
        Func<TKey?> genKey = context.Generate<TKey>;
        Func<TValue?> genVal = context.Generate<TValue>;

        // If we got the BCL Dictionary, avoid interface dispatch in the hot loop
        if (items is Dictionary<TKey, TValue> dict)
        {
            dict.EnsureCapacity(target);

            // Attempts: allow a small overrun to cope with duplicate keys / nulls
            int attemptsLeft = target + 8; // small slack; tune if needed

            while (dict.Count < target && attemptsLeft-- > 0)
            {
                TKey? key = genKey();

                if (key is null)
                    continue;

                TValue? val = genVal();
                if (val is null)
                    continue;

                _ = dict.TryAdd(key, val);
            }

            return dict;
        }
        else
        {
            // Custom IDictionary<,> fallback (can�t avoid interface calls)
            // If it *happens* to be a Dictionary under the hood, ensure capacity
            if (items is Dictionary<TKey, TValue> d2)
                d2.EnsureCapacity(target);

            int attemptsLeft = target + 8;

            while (items.Count < target && attemptsLeft-- > 0)
            {
                TKey? key = genKey();
                if (key is null)
                    continue;

                TValue? val = genVal();
                if (val is null)
                    continue;

                // Using ContainsKey+indexer would be two lookups; Add throws on dup, so try/catch is ok but slower.
                // Interface doesn�t expose TryAdd; if dups are possible, you can guard:
                if (items is Dictionary<TKey, TValue> d3)
                {
                    _ = d3.TryAdd(key, val);
                }
                else
                {
                    // Best-effort: avoid exceptions where possible
                    items.TryAdd(key, val);
                }
            }

            return items;
        }
    }
}