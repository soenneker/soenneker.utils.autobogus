using Soenneker.Reflection.Cache.Properties;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Enums;

internal sealed class EnumValuesGenerator : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        // Soenneker.Gen.EnumValues emits: public static IReadOnlyList<T> List => __values;
        IEnumerable? values = GetList(context.CachedType!);

        if (values == null)
            return null!;

        object? selectedValue = null;

        // IReadOnlyList<T> has Count and indexer but does not implement non-generic IList
        if (values is IList list)
        {
            if (list.Count == 0)
                return null!;

            selectedValue = list[Random.Shared.Next(list.Count)];
        }
        else
        {
            var valueList = new List<object>();

            foreach (object? item in values)
            {
                if (item != null)
                    valueList.Add(item);
            }

            if (valueList.Count == 0)
                return null!;

            selectedValue = valueList[Random.Shared.Next(valueList.Count)];
        }

        return selectedValue ?? null!;
    }

    /// <summary>
    /// Gets the static List property emitted by Soenneker.Gen.EnumValues (IReadOnlyList of all instances).
    /// </summary>
    private static IEnumerable? GetList(CachedType cachedType)
    {
        CachedProperty? listProperty = cachedType.GetCachedProperty("List");

        if (listProperty?.PropertyInfo?.GetValue(null) is IEnumerable values)
            return values;

        return null;
    }
}
