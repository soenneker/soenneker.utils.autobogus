using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using Soenneker.Utils.AutoBogus.Generators.Types.Enums;

namespace Soenneker.Utils.AutoBogus.Services;

/// <summary>
/// This lives on the binder, so if you create a new Binder, you will lose the cache.
/// </summary>
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
        {typeof(Half), new Lazy<IAutoFakerGenerator>(() => new HalfGenerator())},
        {typeof(BigInteger), new Lazy<IAutoFakerGenerator>(() => new BigIntegerGenerator())},
        {typeof(DateTimeOffset), new Lazy<IAutoFakerGenerator>(() => new DateTimeOffsetGenerator())},
        {typeof(DateOnly), new Lazy<IAutoFakerGenerator>(() => new DateOnlyGenerator())},
        {typeof(TimeOnly), new Lazy<IAutoFakerGenerator>(() => new TimeOnlyGenerator())},
        {typeof(TimeSpan), new Lazy<IAutoFakerGenerator>(() => new TimeSpanGenerator())},
        {typeof(IPAddress), new Lazy<IAutoFakerGenerator>(() => new IpAddressGenerator())},
        {typeof(MemoryStream), new Lazy<IAutoFakerGenerator>(() => new MemoryStreamGenerator())},
        {typeof(Exception), new Lazy<IAutoFakerGenerator>(() => new ExceptionGenerator())},
        {typeof(WeakReference), new Lazy<IAutoFakerGenerator>(() => new WeakReferenceGenerator())},
        {typeof(Stream), new Lazy<IAutoFakerGenerator>(() => new StreamGenerator())}
    };

    private readonly Lazy<Dictionary<int, Lazy<IAutoFakerGenerator>>> _cachedFundamentalGeneratorsByInt;

    private readonly ConcurrentDictionary<int, IAutoFakerGenerator> _cachedGenerators = [];

    private static readonly Lazy<IAutoFakerGenerator> _intellenumGenerator =
        new(() => new IntellenumGenerator());

    private static readonly Lazy<IAutoFakerGenerator> _smartEnumGenerator =
        new(() => new SmartEnumGenerator());

    internal GeneratorService()
    {
        _cachedFundamentalGeneratorsByInt = new Lazy<Dictionary<int, Lazy<IAutoFakerGenerator>>>(() =>
        {
            var hashCodesMap = new Dictionary<int, Lazy<IAutoFakerGenerator>>(_cachedFundamentalGenerators.Count);

            foreach (KeyValuePair<Type, Lazy<IAutoFakerGenerator>> kvp in _cachedFundamentalGenerators)
            {
                int keyHashCode = kvp.Key.GetHashCode();
                hashCodesMap[keyHashCode] = kvp.Value;
            }

            return hashCodesMap;
        });

    }

    public IAutoFakerGenerator? GetFundamentalGenerator(CachedType cachedType)
    {
        return _cachedFundamentalGeneratorsByInt.Value.GetValueOrDefault(cachedType.CacheKey!.Value)?.Value;
    }

    public IAutoFakerGenerator? GetGenerator(CachedType cachedType)
    {
        return _cachedGenerators.GetValueOrDefault(cachedType.CacheKey!.Value);
    }

    public void SetGenerator(CachedType cachedType, IAutoFakerGenerator generator)
    {
        _cachedGenerators.TryAdd(cachedType.CacheKey!.Value, generator);
    }

    // Not really happy with this pattern...

    internal static IAutoFakerGenerator GetIntellenumGenerator()
    {
        return _intellenumGenerator.Value;
    }

    internal static IAutoFakerGenerator GetSmartEnumGenerator()
    {
        return _smartEnumGenerator.Value;
    }

    /// <summary>
    /// For test only!
    /// </summary>
    /// <returns></returns>
    public static List<Type> GetSupportedFundamentalTypes()
    {
        var supportedTypes = new List<Type>(_cachedFundamentalGenerators.Count);

        foreach (KeyValuePair<Type, Lazy<IAutoFakerGenerator>> kvp in _cachedFundamentalGenerators)
        {
            // Abstract types
            if (kvp.Key == typeof(Stream))
                continue;

            supportedTypes.Add(kvp.Key);
        }

        return supportedTypes;
    }

    public void Clear()
    {
        _cachedGenerators.Clear();
    }
}