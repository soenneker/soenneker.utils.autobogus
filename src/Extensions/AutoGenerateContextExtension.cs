using System;
using System.Collections.Generic;
using System.Linq;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

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
        count ??= context.Config.RepeatCount;

        List<TType> items = GenerateMany<TType>(context, count.Value, false);

        return items;
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

        List<TType> items = GenerateMany<TType>(context, count.Value, true);

        return items;
    }

    internal static List<TType> GenerateMany<TType>(AutoFakerContext context, int count, bool unique, int maxAttempts = 1, Func<TType?>? generate = null)
    {
        generate ??= context.Generate<TType>;

        if (count <= 0)
            return new List<TType>();

        if (!unique)
        {
            var items = new List<TType>(count);

            for (var i = 0; i < count; i++)
            {
                TType? item = generate.Invoke();

                if (item != null)
                    items.Add(item);
            }

            return items;
        }
        else
        {
            var hashSet = new HashSet<TType>();
            var attempts = 0;
            int totalAttempts = count + maxAttempts - 1;

            while (hashSet.Count < count && attempts < totalAttempts)
            {
                TType? item = generate.Invoke();

                if (item != null)
                    hashSet.Add(item);

                attempts++;
            }
            return hashSet.ToList();
        }
    }
}