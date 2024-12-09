using System;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Generators.Types;
using Soenneker.Utils.AutoBogus.Generators.Types.Immutables;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class CachedTypeService
{
    internal static readonly Lazy<CachedType> TypeGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(TypeGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> EnumerableGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(EnumerableGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ListGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(ListGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> SetGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(SetGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ReadOnlyDictionaryGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(ReadOnlyDictionaryGenerator<,>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> NullableGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(NullableGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> EnumGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(EnumGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ArrayGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(ArrayGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> DictionaryGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(DictionaryGenerator<,>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> CollectionGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(CollectionGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ReadOnlyCollectionGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(ReadOnlyCollectionGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ImmutableListGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(ImmutableListGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ImmutableArrayGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(ImmutableArrayGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> WeakReferenceGenerator = new(() => StaticCacheService.Cache.GetCachedType(typeof(WeakReferenceGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    internal static readonly Lazy<CachedType> IDictionary = new(() => StaticCacheService.Cache.GetCachedType(typeof(IDictionary<,>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> IEnumerable = new(() => StaticCacheService.Cache.GetCachedType(typeof(IEnumerable<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> AutoFaker = new(() => StaticCacheService.Cache.GetCachedType(typeof(AutoFaker)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> Object = new(() => StaticCacheService.Cache.GetCachedType(typeof(object)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
}