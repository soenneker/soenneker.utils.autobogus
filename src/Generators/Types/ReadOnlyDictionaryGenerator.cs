using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class ReadOnlyDictionaryGenerator<TKey, TValue> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        IAutoFakerGenerator generator = new DictionaryGenerator<TKey, TValue>();

        Type generateType = context.GenerateType;

        if (context.CachedType.IsInterface)
            generateType = typeof(Dictionary<TKey, TValue>);

        // Generate a standard dictionary and create the read only dictionary
        var items = generator.Generate(context) as IDictionary<TKey, TValue>;

        return Activator.CreateInstance(generateType, new[] { items });
    }
}