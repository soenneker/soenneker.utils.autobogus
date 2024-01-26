using Soenneker.Reflection.Cache.Types;
using System;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class CachedTypeExtension
{
    public static Type[] GetAddMethodArgumentTypes(this CachedType type)
    {
        if (!type.IsGenericType)
            return [typeof(object)];

        return type.GetGenericArguments()!;
    }
}