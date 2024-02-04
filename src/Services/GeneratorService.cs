using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class GeneratorService
{
    private static readonly Lazy<Dictionary<Type, Lazy<IAutoFakerGenerator>>> _cachedFundamentalGenerators = new(() =>
    {
        var generatorMap = new Dictionary<Type, Lazy<IAutoFakerGenerator>>
    {
        { typeof(string), new Lazy<IAutoFakerGenerator>(() => new StringGenerator(), true) },
        { typeof(int), new Lazy<IAutoFakerGenerator>(() => new IntGenerator(), true) },
        { typeof(bool), new Lazy<IAutoFakerGenerator>(() => new BoolGenerator(), true) },
        { typeof(double), new Lazy<IAutoFakerGenerator>(() => new DoubleGenerator(), true) },
        { typeof(DateTime), new Lazy<IAutoFakerGenerator>(() => new DateTimeGenerator(), true) },
        { typeof(byte), new Lazy<IAutoFakerGenerator>(() => new ByteGenerator(), true) },
        { typeof(char), new Lazy<IAutoFakerGenerator>(() => new CharGenerator(), true) },
        { typeof(decimal), new Lazy<IAutoFakerGenerator>(() => new DecimalGenerator(), true) },
        { typeof(float), new Lazy<IAutoFakerGenerator>(() => new FloatGenerator(), true) },
        { typeof(long), new Lazy<IAutoFakerGenerator>(() => new LongGenerator(), true) },
        { typeof(Guid), new Lazy<IAutoFakerGenerator>(() => new GuidGenerator(), true) },
        { typeof(sbyte), new Lazy<IAutoFakerGenerator>(() => new SByteGenerator(), true) },
        { typeof(short), new Lazy<IAutoFakerGenerator>(() => new ShortGenerator(), true) },
        { typeof(uint), new Lazy<IAutoFakerGenerator>(() => new UIntGenerator(), true) },
        { typeof(ulong), new Lazy<IAutoFakerGenerator>(() => new ULongGenerator(), true) },
        { typeof(Uri), new Lazy<IAutoFakerGenerator>(() => new UriGenerator(), true) },
        { typeof(ushort), new Lazy<IAutoFakerGenerator>(() => new UShortGenerator(), true) },
        { typeof(DateTimeOffset), new Lazy<IAutoFakerGenerator>(() => new DateTimeOffsetGenerator(), true) },
        { typeof(DateOnly), new Lazy<IAutoFakerGenerator>(() => new DateOnlyGenerator(), true) },
        { typeof(TimeOnly), new Lazy<IAutoFakerGenerator>(() => new TimeOnlyGenerator(), true) },
        { typeof(IPAddress), new Lazy<IAutoFakerGenerator>(() => new IpAddressGenerator(), true) },
    };

        return generatorMap;
    });
    private static readonly Lazy<Dictionary<int, Lazy<IAutoFakerGenerator>>> _cachedFundamentalGeneratorsByInt = new(() =>
    {
        Dictionary<int, Lazy<IAutoFakerGenerator>> hashCodesMap = _cachedFundamentalGenerators.Value.ToDictionary(
            kvp => kvp.Key.GetHashCode(),
            kvp => kvp.Value
        );

        return hashCodesMap;
    }, true);

    private static readonly ConcurrentDictionary<int, IAutoFakerGenerator> _cachedGenerators = [];

    public static IAutoFakerGenerator? GetFundamentalGenerator(CachedType cachedType)
    {
        int hashCode = cachedType.CacheKey.Value;

        IAutoFakerGenerator? result = _cachedFundamentalGeneratorsByInt.Value.GetValueOrDefault(hashCode)?.Value;
        return result;
    }

    public static IAutoFakerGenerator? GetGenerator(CachedType cachedType)
    {
        int hashCode = cachedType.CacheKey.Value;

        IAutoFakerGenerator? result = _cachedGenerators.GetValueOrDefault(hashCode);

        return result;
    }

    public static void SetGenerator(CachedType cachedType, IAutoFakerGenerator generator)
    {
        int hashCode = cachedType.CacheKey.Value;

        _cachedGenerators.TryAdd(hashCode, generator);
    }

    /// <summary>
    /// For test only!
    /// </summary>
    /// <returns></returns>
    public static List<Type> GetSupportedFundamentalTypes()
    {
        return _cachedFundamentalGenerators.Value.Select(c => c.Key).ToList();
    }

    public static void Clear()
    {
        _cachedGenerators.Clear();
    }
}