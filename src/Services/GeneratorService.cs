using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Soenneker.Utils.AutoBogus.Services;

internal static class GeneratorService
{
    private static readonly Lazy<Dictionary<Type, IAutoFakerGenerator>> _cachedGenerators = new(() =>
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

    private static readonly Lazy<Dictionary<int, IAutoFakerGenerator>> _cachedGeneratorsByInt = new(() =>
    {
        var hashCodesMap = _cachedGenerators.Value.ToDictionary(
            kvp => kvp.Key.GetHashCode(),
            kvp => kvp.Value
        );

        return hashCodesMap;
    }, true);

    public static IAutoFakerGenerator GetFundamentalGenerator(CachedType cachedType)
    {
        int hashCode = cachedType.CacheKey.Value;

        return _cachedGeneratorsByInt.Value.GetValueOrDefault(hashCode);
    }

    public static List<Type> GetSupportedFundamentalTypes()
    {
        return _cachedGenerators.Value.Select(c => c.Key).ToList();
    }
}