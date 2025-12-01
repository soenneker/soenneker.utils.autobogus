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

    /// <summary>
    /// Gets a built-in generator for fundamental types (primitives, strings, dates, etc.) if one exists.
    /// </summary>
    /// <param name="cachedType">The cached type information for the type to get a generator for.</param>
    /// <returns>The generator for the type if it's a supported fundamental type; otherwise, <see langword="null"/>.</returns>
    /// <remarks>
    /// Fundamental types include primitives (int, bool, etc.), strings, DateTime, Guid, and other common .NET types.
    /// This method only returns generators for these built-in types, not custom generators registered via <see cref="SetGenerator"/>.
    /// </remarks>
    public IAutoFakerGenerator? GetFundamentalGenerator(CachedType cachedType)
    {
        return _cachedFundamentalGeneratorsByInt.Value.GetValueOrDefault(cachedType.CacheKey!.Value)?.Value;
    }

    /// <summary>
    /// Gets a custom generator that was previously registered for the specified cached type.
    /// </summary>
    /// <param name="cachedType">The cached type information for the type to get a generator for.</param>
    /// <returns>The custom generator if one was registered via <see cref="SetGenerator"/>; otherwise, <see langword="null"/>.</returns>
    /// <remarks>
    /// This method retrieves generators that were explicitly registered using <see cref="SetGenerator"/>.
    /// It does not return fundamental type generators; use <see cref="GetFundamentalGenerator"/> for those.
    /// </remarks>
    public IAutoFakerGenerator? GetGenerator(CachedType cachedType)
    {
        return _cachedGenerators.GetValueOrDefault(cachedType.CacheKey!.Value);
    }

    /// <summary>
    /// Registers a custom generator to use for generating instances of the specified type.
    /// </summary>
    /// <param name="cachedType">The cached type information for the type to register a generator for.</param>
    /// <param name="generator">The custom generator implementation that will be used to generate instances of this type.</param>
    /// <remarks>
    /// Once registered, this generator will be used for all generation requests of the specified type.
    /// Custom generators take precedence over default generation logic. This is useful for types that require
    /// special handling or complex initialization logic.
    /// </remarks>
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

    /// <summary>
    /// Removes all custom generators that were registered via <see cref="SetGenerator"/>, resetting to default generation behavior.
    /// </summary>
    /// <remarks>
    /// This does not affect fundamental type generators, which are always available. Only custom generators are cleared.
    /// </remarks>
    public void Clear()
    {
        _cachedGenerators.Clear();
    }
}