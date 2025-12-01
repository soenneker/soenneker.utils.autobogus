using System.Collections.Generic;
using Bogus;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Abstract;

/// <summary>
/// Defines the contract for generating fake data instances of a specific type <typeparamref name="TType"/>.
/// </summary>
/// <typeparam name="TType">The type of instance to generate. Must be a reference type (class).</typeparam>
/// <remarks>
/// This interface extends <see cref="Bogus.Faker{TType}"/> with automatic generation capabilities, allowing you to generate
/// instances of <typeparamref name="TType"/> without manually defining rules for each property. It supports rule sets for
/// different generation scenarios and integrates with the AutoBogus configuration system.
/// </remarks>
public interface IAutoFaker<TType> where TType : class
{
    /// <summary>
    /// Gets or sets the configuration that controls how instances are generated, including recursion depth, tree depth, skip lists, and overrides.
    /// </summary>
    AutoFakerConfig Config { get; set; }

    /// <summary>
    /// Gets or sets the binder instance responsible for creating instances and populating their members with generated values.
    /// </summary>
    /// <remarks>
    /// The binder handles the actual instantiation and population logic. You can provide a custom binder to extend or modify this behavior.
    /// </remarks>
    AutoFakerBinder? Binder { get; set; }

    /// <summary>
    /// Forces initialization of the binder and cache service. This is typically called automatically before generation.
    /// </summary>
    /// <remarks>
    /// This method initializes the <see cref="Binder"/> and internal <see cref="CacheService"/> if they haven't been initialized yet.
    /// It's automatically called before generation, so you typically don't need to call it manually. However, it's available
    /// if you need to access the binder or cache service before the first generation call.
    /// </remarks>
    void Initialize();

    /// <summary>
    /// Generates a single instance of <typeparamref name="TType"/> with all properties and fields populated with fake data.
    /// </summary>
    /// <param name="ruleSets">An optional comma-separated list of rule set names to use for this generation. Rule sets allow you to define different generation behaviors for different scenarios (e.g., "default", "minimal", "complete").</param>
    /// <returns>A new instance of <typeparamref name="TType"/> with all members populated with generated values according to the specified rule sets.</returns>
    /// <remarks>
    /// This method automatically creates an instance and populates all public and private properties and fields with appropriate fake data.
    /// Nested objects are also generated recursively according to the configuration settings. Rule sets allow you to customize
    /// which members are generated for different use cases.
    /// </remarks>
    TType Generate(string? ruleSets = null);

    /// <summary>
    /// Generates a collection containing the specified number of instances of <typeparamref name="TType"/>.
    /// </summary>
    /// <param name="count">The number of instances to generate in the collection.</param>
    /// <param name="ruleSets">An optional comma-separated list of rule set names to use for this generation. Rule sets allow you to define different generation behaviors for different scenarios.</param>
    /// <returns>A <see cref="List{TType}"/> containing <paramref name="count"/> generated instances, each with its own set of random values.</returns>
    /// <remarks>
    /// Each instance in the collection is independently generated with its own set of random values according to the specified rule sets.
    /// </remarks>
    List<TType> Generate(int count, string? ruleSets = null);

    /// <summary>
    /// Populates all writable properties and fields of the provided instance with generated values.
    /// </summary>
    /// <param name="instance">The existing instance to populate with generated values. Must not be <see langword="null"/>.</param>
    /// <param name="ruleSets">An optional comma-separated list of rule set names to use for this population. Rule sets allow you to define which members should be populated.</param>
    /// <remarks>
    /// This method is useful when you already have an instance (perhaps created with specific constructor parameters) and want to
    /// populate its properties and fields with generated data. It respects rule sets, so you can control which members are populated.
    /// Members that are handled by rule sets will be skipped during population.
    /// </remarks>
    void Populate(TType instance, string? ruleSets = null);
}

