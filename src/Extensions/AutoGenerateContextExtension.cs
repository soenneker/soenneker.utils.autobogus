using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System;
using System.Collections.Generic;

namespace Soenneker.Utils.AutoBogus.Extensions;

/// <summary>
/// A class extending the <see cref="AutoFakerContext"/> class.
/// </summary>
public static class AutoGenerateContextExtension
{
    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The instance type to generate.</typeparam>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the current generate request.</param>
    /// <returns>The generated instance.</returns>
    public static TType? Generate<TType>(this AutoFakerContext context)
    {
        CachedType cachedType = context.CacheService.Cache.GetCachedType(typeof(TType));

        // Set the generate type for the current request
        context.Setup(cachedType);

        // Get the type generator and return a value
        IAutoFakerGenerator generator = AutoFakerGeneratorFactory.GetGenerator(context);
        object? generatedInstance = generator.Generate(context);

        if (generatedInstance == null)
            return default;

        return (TType?)generatedInstance;
    }

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The instance type to generate.</typeparam>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the current generate request.</param>
    /// <param name="count">The number of instances to generate.</param>
    /// <returns>The generated collection of instances.</returns>
    public static List<TType> GenerateMany<TType>(this AutoFakerContext context, int? count = null)
    {
        // When RecursiveDepth is 0 and we're generating nested items (stackCount > 0), return empty list
        if (context.Config.RecursiveDepth == 0 && context.TypesStack.Count > 0)
        {
            return [];
        }

        count ??= context.Config.RepeatCount;

        return GenerateMany<TType>(context, count.Value, false);
    }

    /// <summary>
    /// Generates a collection of unique instances of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The instance type to generate.</typeparam>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the current generate request.</param>
    /// <param name="count">The number of instances to generate.</param>
    /// <returns>The generated collection of unique instances.</returns>
    public static List<TType> GenerateUniqueMany<TType>(this AutoFakerContext context, int? count = null)
    {
        count ??= context.Config.RepeatCount;

        return GenerateMany<TType>(context, count.Value, true);
    }

    internal static List<TType> GenerateMany<TType>(AutoFakerContext context, int count, bool unique, int maxAttempts = 1, Func<TType?>? generate = null,
        IEqualityComparer<TType>? comparer = null)
    {
        if (count <= 0)
            return [];

        Func<TType?> gen = generate ?? context.Generate<TType>;

        if (!unique)
        {
            TType[] buf = GC.AllocateUninitializedArray<TType>(count);
            var written = 0;

            for (var i = 0; i < count; i++)
            {
                TType? item = gen();
                if (item is not null)
                    buf[written++] = item;
            }

            if (written == 0)
                return [];

            var result = new List<TType>(written);

            result.AddRange(buf.AsSpan(0, written));

            return result;
        }

        int totalAttempts = count + Math.Max(0, maxAttempts - 1);

        HashSet<TType> set = comparer is null ? [] : new HashSet<TType>(comparer);
        set.EnsureCapacity(count);

        var attempts = 0;
        while (set.Count < count && attempts < totalAttempts)
        {
            TType? item = gen();
            if (item is not null)
                _ = set.Add(item);

            attempts++;
        }

        return set.Count == 0 ? [] : [..set];
    }
}