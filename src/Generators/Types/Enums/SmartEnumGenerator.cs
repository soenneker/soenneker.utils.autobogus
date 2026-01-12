using Soenneker.Reflection.Cache.Properties;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System;
using System.Collections;
using System.Reflection;
using Soenneker.Reflection.Cache.Constructors;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Enums;

internal sealed class SmartEnumGenerator : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        Type? declaringType = context.CachedType.Type;
        if (declaringType == null)
            return null!;

        // Traverse base types to find "SmartEnum`2"
        while (declaringType != null && declaringType != typeof(object))
        {
            if (declaringType.IsGenericType && declaringType.Name.StartsWith("SmartEnum`2"))
                break;

            declaringType = declaringType.BaseType;
        }

        if (declaringType == null || declaringType == typeof(object))
            return null!;

        CachedType cachedBase = context.CacheService.Cache.GetCachedType(declaringType);
        CachedProperty? propertyInfo = cachedBase.GetCachedProperty("List");

        if (propertyInfo?.PropertyInfo?.GetValue(null) is not IEnumerable values)
            return null!;

        object? selectedValue = null;

        // Fast-path: indexed access without allocations
        if (values is IList list)
        {
            if (list.Count == 0)
                return null!;

            selectedValue = list[Random.Shared.Next(list.Count)];
        }
        else
        {
            // Reservoir sampling: choose a random element in one pass without buffering
            var seen = 0;

            foreach (object? v in values)
            {
                if (v is null)
                    continue;

                seen++;

                if (Random.Shared.Next(seen) == 0)
                    selectedValue = v;
            }

            if (selectedValue is null)
                return null!;
        }

        Type selectedType = selectedValue.GetType();

        // Return directly if it's already the correct type
        if (context.CachedType.Type.IsAssignableFrom(selectedType))
            return selectedValue;

        // Attempt to create an instance of the derived type
        CachedConstructor? ctor = context.CachedType.GetCachedConstructor(typeof(string), typeof(int));
        if (ctor == null)
            return null!;

        // Retrieve Name and Value properties via reflection cache
        CachedType selectedCachedType = context.CacheService.Cache.GetCachedType(selectedType);
        PropertyInfo? nameProperty = selectedCachedType.GetCachedProperty("Name")?.PropertyInfo;
        PropertyInfo? valueProperty = selectedCachedType.GetCachedProperty("Value")?.PropertyInfo;

        if (nameProperty?.GetValue(selectedValue) is string name &&
            valueProperty?.GetValue(selectedValue) is int value)
        {
            return ctor.Invoke(name, value);
        }

        return null!;
    }


}