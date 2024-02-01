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
    private static readonly Lazy<Dictionary<Type, IAutoFakerGenerator>> _cachedFundamentalGenerators = new(() =>
    {
        var generatorMap = new Dictionary<Type, IAutoFakerGenerator>
        {
            {typeof(string), new StringGenerator()},
            {typeof(int), new IntGenerator()},
            {typeof(bool), new BoolGenerator()},
            {typeof(double), new DoubleGenerator()},
            {typeof(DateTime), new DateTimeGenerator()},
            {typeof(byte), new ByteGenerator()},
            {typeof(char), new CharGenerator()},
            {typeof(decimal), new DecimalGenerator()},
            {typeof(float), new FloatGenerator()},
            {typeof(long), new LongGenerator()},
            {typeof(Guid), new GuidGenerator()},
            {typeof(sbyte), new SByteGenerator()},
            {typeof(short), new ShortGenerator()},
            {typeof(uint), new UIntGenerator()},
            {typeof(ulong), new ULongGenerator()},
            {typeof(Uri), new UriGenerator()},
            {typeof(ushort), new UShortGenerator()},
            {typeof(DateTimeOffset), new DateTimeOffsetGenerator()},
            {typeof(DateOnly), new DateOnlyGenerator()},
            {typeof(TimeOnly), new TimeOnlyGenerator()},
            {typeof(IPAddress), new IpAddressGenerator()},
        };

        return generatorMap;
    });

    private static readonly Lazy<Dictionary<int, IAutoFakerGenerator>> _cachedFundamentalGeneratorsByInt = new(() =>
    {
        Dictionary<int, IAutoFakerGenerator> hashCodesMap = _cachedFundamentalGenerators.Value.ToDictionary(
            kvp => kvp.Key.GetHashCode(),
            kvp => kvp.Value
        );

        return hashCodesMap;
    }, true);

    private static readonly ConcurrentDictionary<int, IAutoFakerGenerator> _cachedGenerators = [];

    public static IAutoFakerGenerator? GetFundamentalGenerator(CachedType cachedType)
    {
        int hashCode = cachedType.CacheKey.Value;

        IAutoFakerGenerator? result = _cachedFundamentalGeneratorsByInt.Value.GetValueOrDefault(hashCode);
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