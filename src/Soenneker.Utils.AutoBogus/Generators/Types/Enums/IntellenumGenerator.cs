using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System;
using System.Collections;
using Soenneker.Reflection.Cache.Methods;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Enums;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class IntellenumGenerator : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        CachedMethod? listMethod = context.CachedType.GetCachedMethod("List");

        if (listMethod == null)
            return null!;

        object? result = listMethod.Invoke(null);

        if (result is not IEnumerable values)
            return null!;

        if (values is IList list)
        {
            if (list.Count == 0)
                return null!;

            return list[Random.Shared.Next(list.Count)]!;
        }

        object? selected = null;
        var seen = 0;

        foreach (object? item in values)
        {
            if (item is null)
                continue;

            seen++;
            if (Random.Shared.Next(seen) == 0)
                selected = item;
        }

        return selected ?? null!;
    }
}
