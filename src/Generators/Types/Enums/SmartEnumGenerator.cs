using Soenneker.Reflection.Cache.Properties;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System;
using System.Collections;
using System.Linq;
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

        // Convert IEnumerable to a more efficient indexed collection (avoid multiple enumerations)
        object[] valueArray = values.Cast<object>().ToArray();
        if (valueArray.Length == 0)
            return null!;

        // Select a random item using a single call to Random
        object selectedValue = valueArray[Random.Shared.Next(valueArray.Length)];

        Type selectedType = selectedValue.GetType();

        // Return directly if it's already the correct type
        if (context.CachedType.Type.IsAssignableFrom(selectedType))
            return selectedValue;

        // Attempt to create an instance of the derived type
        CachedConstructor? ctor = context.CachedType.GetCachedConstructor([typeof(string), typeof(int)]);
        if (ctor == null)
            return null!;

        // Retrieve Name and Value properties just once
        PropertyInfo? nameProperty = selectedType.GetProperty("Name");
        PropertyInfo? valueProperty = selectedType.GetProperty("Value");

        if (nameProperty?.GetValue(selectedValue) is string name &&
            valueProperty?.GetValue(selectedValue) is int value)
        {
            return ctor.Invoke([name, value]);
        }

        return null!;
    }


}