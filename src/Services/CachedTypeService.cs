using System;
using System.Collections.Generic;
using Soenneker.Reflection.Cache;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Generators.Types;
using Soenneker.Utils.AutoBogus.Generators.Types.Immutables;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class CachedTypeService
{
    public static ReflectionCache ReflectionCache { get; set; } = new ReflectionCache();

    internal static readonly Lazy<CachedType> TypeGenerator = new(() => ReflectionCache.GetCachedType(typeof(TypeGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> EnumerableGenerator = new(() => ReflectionCache.GetCachedType(typeof(EnumerableGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ListGenerator = new(() => ReflectionCache.GetCachedType(typeof(ListGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> SetGenerator = new(() => ReflectionCache.GetCachedType(typeof(SetGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ReadOnlyDictionaryGenerator = new(() => ReflectionCache.GetCachedType(typeof(ReadOnlyDictionaryGenerator<,>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> NullableGenerator = new(() => ReflectionCache.GetCachedType(typeof(NullableGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> EnumGenerator = new(() => ReflectionCache.GetCachedType(typeof(EnumGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ArrayGenerator = new(() => ReflectionCache.GetCachedType(typeof(ArrayGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> DictionaryGenerator = new(() => ReflectionCache.GetCachedType(typeof(DictionaryGenerator<,>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> CollectionGenerator = new(() => ReflectionCache.GetCachedType(typeof(CollectionGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ReadOnlyCollectionGenerator = new(() => ReflectionCache.GetCachedType(typeof(ReadOnlyCollectionGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> ImmutableListGenerator = new(() => ReflectionCache.GetCachedType(typeof(ImmutableListGenerator<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    internal static readonly Lazy<CachedType> IDictionary = new(() => ReflectionCache.GetCachedType(typeof(IDictionary<,>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> IEnumerable = new(() => ReflectionCache.GetCachedType(typeof(IEnumerable<>)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> AutoFaker = new(() => ReflectionCache.GetCachedType(typeof(AutoFaker)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    internal static readonly Lazy<CachedType> Object = new(() => ReflectionCache.GetCachedType(typeof(object)), System.Threading.LazyThreadSafetyMode.PublicationOnly);
}