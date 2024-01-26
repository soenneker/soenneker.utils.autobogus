using System;
using System.Collections.Generic;
using System.Linq;
using Soenneker.Utils.AutoBogus.Config;
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
    public static TType Generate<TType>(this AutoFakerContext context)
    {
        if (context != null)
        {
            // Set the generate type for the current request
            context.GenerateType = typeof(TType);

            // Get the type generator and return a value
            IAutoFakerGenerator generator = GeneratorFactory.GetGenerator(context);
            return (TType)generator.Generate(context);
        }

        return default;
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
        var items = new List<TType>();

        if (context != null)
        {
            GenerateMany(context, count, items, false);
        }

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
        var items = new List<TType>();

        if (context != null)
        {
            GenerateMany(context, count, items, true);
        }

        return items;
    }
    
    /// <summary>
    /// Populates the provided instance with generated values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to populate.</typeparam>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the current generate request.</param>
    /// <param name="instance">The instance to populate.</param>
    public static void Populate<TType>(this AutoFakerContext context, TType instance)
    {
        if (context != null)
        {
            context.FakerBinder.PopulateInstance<TType>(instance, context);
        }
    }

    internal static void GenerateMany<TType>(AutoFakerContext context, int? count, List<TType> items, bool unique, int attempt = 1, Func<TType>? generate = null)
    {
        while (true)
        {
            // Apply any defaults
            if (count == null)
            {
                count = context.FakerConfig.RepeatCount.Invoke(context);
            }

            generate ??= context.Generate<TType>;

            // Generate a list of items
            int? required = count - items.Count;

            for (var index = 0; index < required; index++)
            {
                TType? item = generate.Invoke();

                // Ensure the generated value is not null (which means the type couldn't be generated)
                if (item != null)
                {
                    items.Add(item);
                }
            }

            if (unique)
            {
                // Remove any duplicates and generate more to match the required count
                List<TType> filtered = items.Distinct().ToList();

                if (filtered.Count < count)
                {
                    // To maintain the items reference, clear and reapply the filtered list
                    items.Clear();
                    items.AddRange(filtered);

                    // Only continue to generate more if the attempts threshold is not reached
                    if (attempt < AutoFakerConfig.GenerateAttemptsThreshold)
                    {
                        attempt += 1;
                        continue;
                    }
                }
            }

            break;
        }
    }
}