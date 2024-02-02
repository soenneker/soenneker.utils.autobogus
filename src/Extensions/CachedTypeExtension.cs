using Soenneker.Reflection.Cache.Types;
using System;
using System.Dynamic;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class CachedTypeExtension
{
    public static Type[] GetAddMethodArgumentTypes(this CachedType type)
    {
        if (!type.IsGenericType)
            return [typeof(object)];

        return type.GetGenericArguments()!;
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