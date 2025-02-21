using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Methods;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Enums;

internal sealed class IntellenumGenerator : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        CachedMethod? listMethod = context.CachedType.GetCachedMethod("List");

        if (listMethod == null)
            return null!;

        object result = listMethod.Invoke(null, null);

        if (result is not IEnumerable values)
            return null!;

        // Convert to a list and pick a random value
        var valueList = new List<object>();

        foreach (object? item in values)
        {
            valueList.Add(item);
        }

        if (valueList.Count == 0)
            return null!;

        return valueList[Random.Shared.Next(valueList.Count)];
    }
}