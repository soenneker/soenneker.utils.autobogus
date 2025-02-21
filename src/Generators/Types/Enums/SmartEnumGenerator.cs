using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Properties;
using Soenneker.Reflection.Cache.Types;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Enums;

internal sealed class SmartEnumGenerator : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        Type? declaringType = context.CachedType.Type.BaseType.BaseType;

        CachedType cachedBase = context.CacheService.Cache.GetCachedType(declaringType);

        CachedProperty? propertyInfo = cachedBase.GetCachedProperty("List");

        object? property = propertyInfo.PropertyInfo.GetValue(null);

        if (property is not IEnumerable values)
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