using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Soenneker.Utils.AutoBogus.Services;

internal sealed class GeneratorService
{
    // I believe this can be static because there aren't context/config adjustments
    private static readonly Dictionary<Type, Lazy<IAutoFakerGenerator>> _cachedFundamentalGenerators = new()
    {
        {typeof(string), new Lazy<IAutoFakerGenerator>(() => new StringGenerator())},
        {typeof(int), new Lazy<IAutoFakerGenerator>(() => new IntGenerator())},
        {typeof(bool), new Lazy<IAutoFakerGenerator>(() => new BoolGenerator())},
        {typeof(double), new Lazy<IAutoFakerGenerator>(() => new DoubleGenerator())},
        {typeof(DateTime), new Lazy<IAutoFakerGenerator>(() => new DateTimeGenerator())},
        {typeof(byte), new Lazy<IAutoFakerGenerator>(() => new ByteGenerator())},
        {typeof(char), new Lazy<IAutoFakerGenerator>(() => new CharGenerator())},
        {typeof(decimal), new Lazy<IAutoFakerGenerator>(() => new DecimalGenerator())},
        {typeof(float), new Lazy<IAutoFakerGenerator>(() => new FloatGenerator())},
        {typeof(long), new Lazy<IAutoFakerGenerator>(() => new LongGenerator())},
        {typeof(Guid), new Lazy<IAutoFakerGenerator>(() => new GuidGenerator())},
        {typeof(sbyte), new Lazy<IAutoFakerGenerator>(() => new SByteGenerator())},
        {typeof(short), new Lazy<IAutoFakerGenerator>(() => new ShortGenerator())},
        {typeof(uint), new Lazy<IAutoFakerGenerator>(() => new UIntGenerator())},
        {typeof(ulong), new Lazy<IAutoFakerGenerator>(() => new ULongGenerator())},
        {typeof(Uri), new Lazy<IAutoFakerGenerator>(() => new UriGenerator())},
        {typeof(ushort), new Lazy<IAutoFakerGenerator>(() => new UShortGenerator())},
        {typeof(DateTimeOffset), new Lazy<IAutoFakerGenerator>(() => new DateTimeOffsetGenerator())},
        {typeof(DateOnly), new Lazy<IAutoFakerGenerator>(() => new DateOnlyGenerator())},
        {typeof(TimeOnly), new Lazy<IAutoFakerGenerator>(() => new TimeOnlyGenerator())},
        {typeof(IPAddress), new Lazy<IAutoFakerGenerator>(() => new IpAddressGenerator())},
    };

    private readonly Lazy<Dictionary<int, Lazy<IAutoFakerGenerator>>> _cachedFundamentalGeneratorsByInt;

    private readonly ConcurrentDictionary<int, IAutoFakerGenerator> _cachedGenerators = [];

    internal GeneratorService()
    {
        _cachedFundamentalGeneratorsByInt = new Lazy<Dictionary<int, Lazy<IAutoFakerGenerator>>>(() =>
        {
            Dictionary<int, Lazy<IAutoFakerGenerator>> hashCodesMap = _cachedFundamentalGenerators.ToDictionary(
                kvp => kvp.Key.GetHashCode(),
                kvp => kvp.Value
            );

            return hashCodesMap;
        });
    }

    public IAutoFakerGenerator? GetFundamentalGenerator(CachedType cachedType)
    {
        IAutoFakerGenerator? result = _cachedFundamentalGeneratorsByInt.Value.GetValueOrDefault(cachedType.CacheKey!.Value)?.Value;
        return result;
    }

    public IAutoFakerGenerator? GetGenerator(CachedType cachedType)
    {
        IAutoFakerGenerator? result = _cachedGenerators.GetValueOrDefault(cachedType.CacheKey!.Value);
        return result;
    }

    public void SetGenerator(CachedType cachedType, IAutoFakerGenerator generator)
    {
        _cachedGenerators.TryAdd(cachedType.CacheKey!.Value, generator);
    }

    /// <summary>
    /// For test only!
    /// </summary>
    /// <returns></returns>
    public static List<Type> GetSupportedFundamentalTypes()
    {
        return _cachedFundamentalGenerators.Select(c => c.Key).ToList();
    }

    public void Clear()
    {
        _cachedGenerators.Clear();
    }
}