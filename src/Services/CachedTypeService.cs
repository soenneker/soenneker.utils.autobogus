using System;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Generators.Types;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class CachedTypeService
{
    internal static readonly Lazy<CachedType> TypeGenerator = new(() => CacheService.Cache.GetCachedType(typeof(TypeGenerator<>)));
    internal static readonly Lazy<CachedType> EnumerableGenerator = new(() => CacheService.Cache.GetCachedType(typeof(EnumerableGenerator<>)));
    internal static readonly Lazy<CachedType> ListGenerator = new(() => CacheService.Cache.GetCachedType(typeof(ListGenerator<>)));
    internal static readonly Lazy<CachedType> SetGenerator = new(() => CacheService.Cache.GetCachedType(typeof(SetGenerator<>)));
    internal static readonly Lazy<CachedType> ReadOnlyDictionaryGenerator = new(() => CacheService.Cache.GetCachedType(typeof(ReadOnlyDictionaryGenerator<,>)));
    internal static readonly Lazy<CachedType> NullableGenerator = new(() => CacheService.Cache.GetCachedType(typeof(NullableGenerator<>)));
    internal static readonly Lazy<CachedType> EnumGenerator = new(() => CacheService.Cache.GetCachedType(typeof(EnumGenerator<>)));
    internal static readonly Lazy<CachedType> ArrayGenerator = new(() => CacheService.Cache.GetCachedType(typeof(ArrayGenerator<>)));
    internal static readonly Lazy<CachedType> DictionaryGenerator = new(() => CacheService.Cache.GetCachedType(typeof(DictionaryGenerator<,>)));

    internal static readonly Lazy<CachedType> IDictionary = new(() => CacheService.Cache.GetCachedType(typeof(IDictionary<,>)));
    internal static readonly Lazy<CachedType> IEnumerable = new(() => CacheService.Cache.GetCachedType(typeof(IEnumerable<>)));
}