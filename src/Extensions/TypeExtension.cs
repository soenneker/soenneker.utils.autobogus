using System;
using System.Dynamic;
using Soenneker.Reflection.Cache.Types;

namespace Soenneker.Utils.AutoBogus.Extensions;

public static class TypeExtension
{
    internal static bool IsExpandoObject(this CachedType type)
    {
        return type.Type == typeof(ExpandoObject);
    }

    internal static bool IsDictionary(this CachedType type)
    {
        return IsGenericType(type, "IDictionary`2");
    }

    internal static bool IsReadOnlyDictionary(this CachedType type)
    {
        return IsGenericType(type, "IReadOnlyDictionary`2");
    }

    internal static bool IsCollection(this CachedType type)
    {
        return IsGenericType(type, "ICollection`1");
    }

    internal static bool IsEnumerable(this CachedType type)
    {
        return IsGenericType(type, "IEnumerable`1");
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