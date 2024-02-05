using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Services;
using System;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class CachedTypeExtension
{
    private static readonly CachedType _object = CacheService.Cache.GetCachedType(typeof(object));

    public static CachedType[] GetAddMethodArgumentTypes(this CachedType type)
    {
        if (!type.IsGenericType)
            return [_object];

        return type.GetCachedGenericArguments()!;
    }

    internal static bool IsCollection(this CachedType type)
    {
        return IsGenericType(type, "ICollection`1");
    }

    private static bool IsGenericType(CachedType cachedType, string interfaceTypeName)
    {
        if (cachedType.Type.Name == interfaceTypeName)
            return true;

        Type[] interfaces = cachedType.GetInterfaces()!;

        foreach (Type i in interfaces)
        {
            if (i.Name == interfaceTypeName)
                return true;
        }

        return false;
    }
}