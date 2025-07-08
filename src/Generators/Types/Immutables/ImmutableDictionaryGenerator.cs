using System.Collections.Generic;
using System.Collections.Immutable;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Immutables;

internal sealed class ImmutableDictionaryGenerator<TKey, TValue> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();

        List<TKey> keys = context.GenerateUniqueMany<TKey>();
        int length = keys.Count;

        for (int i = 0; i < length; i++)
        {
            TKey key = keys[i];
            TValue value = context.Generate<TValue>();

            if (value != null)
            {
                builder[key] = value;
            }
        }

        return builder.ToImmutable();
    }
}