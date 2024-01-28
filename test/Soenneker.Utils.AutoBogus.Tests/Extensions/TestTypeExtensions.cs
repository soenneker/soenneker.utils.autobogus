using Soenneker.Utils.AutoBogus.Services;
using System;

namespace Soenneker.Utils.AutoBogus.Tests.Extensions;

public static class TestTypeExtensions
{
    internal static bool IsEnum(this Type type)
    {
        return CacheService.Cache.GetCachedType(type).IsEnum;
    }

    internal static bool IsAbstract(this Type type)
    {
        return CacheService.Cache.GetCachedType(type).IsAbstract;
    }

    internal static bool IsInterface(this Type type)
    {
        return CacheService.Cache.GetCachedType(type).IsInterface;
    }

    internal static bool IsGenericType(this Type type)
    {
        return CacheService.Cache.GetCachedType(type).IsGenericType;
    }
}