using System;
using System.Collections.Generic;
using Soenneker.Reflection.Cache;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Generators.Types;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class CachedTypeService
{
    public static ReflectionCache ReflectionCache { get; set; } = new ReflectionCache();

    internal static readonly Lazy<CachedType> TypeGenerator = new(() => ReflectionCache.GetCachedType(typeof(TypeGenerator<>)));
    internal static readonly Lazy<CachedType> EnumerableGenerator = new(() => ReflectionCache.GetCachedType(typeof(EnumerableGenerator<>)));
    internal static readonly Lazy<CachedType> ListGenerator = new(() => ReflectionCache.GetCachedType(typeof(ListGenerator<>)));
    internal static readonly Lazy<CachedType> SetGenerator = new(() => ReflectionCache.GetCachedType(typeof(SetGenerator<>)));
    internal static readonly Lazy<CachedType> ReadOnlyDictionaryGenerator = new(() => ReflectionCache.GetCachedType(typeof(ReadOnlyDictionaryGenerator<,>)));
    internal static readonly Lazy<CachedType> NullableGenerator = new(() => ReflectionCache.GetCachedType(typeof(NullableGenerator<>)));
    internal static readonly Lazy<CachedType> EnumGenerator = new(() => ReflectionCache.GetCachedType(typeof(EnumGenerator<>)));
    internal static readonly Lazy<CachedType> ArrayGenerator = new(() => ReflectionCache.GetCachedType(typeof(ArrayGenerator<>)));
    internal static readonly Lazy<CachedType> DictionaryGenerator = new(() => ReflectionCache.GetCachedType(typeof(DictionaryGenerator<,>)));

    internal static readonly Lazy<CachedType> IDictionary = new(() => ReflectionCache.GetCachedType(typeof(IDictionary<,>)));
    internal static readonly Lazy<CachedType> IEnumerable = new(() => ReflectionCache.GetCachedType(typeof(IEnumerable<>)));
    internal static readonly Lazy<CachedType> AutoFaker = new(() => ReflectionCache.GetCachedType(typeof(AutoFaker)));
    internal static readonly Lazy<CachedType> Object = new(() => ReflectionCache.GetCachedType(typeof(object)));
}