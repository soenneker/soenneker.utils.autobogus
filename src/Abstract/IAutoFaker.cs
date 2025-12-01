using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Abstract;

/// <summary>
/// Defines the contract for generating fake data instances of various types.
/// </summary>
/// <remarks>
/// This interface provides methods to generate single instances or collections of fake data for any type.
/// It supports configuration of generation behavior, custom overrides, and integration with the Bogus library for random value generation.
/// </remarks>
public interface IAutoFaker
{
    /// <summary>
    /// Gets the configuration that controls how instances are generated, including recursion depth, tree depth, skip lists, and overrides.
    /// </summary>
    AutoFakerConfig Config { get; }

    /// <summary>
    /// Gets or sets the binder instance responsible for creating instances and populating their members with generated values.
    /// </summary>
    /// <remarks>
    /// The binder handles the actual instantiation and population logic. You can provide a custom binder to extend or modify this behavior.
    /// </remarks>
    AutoFakerBinder? Binder { get; set; }

    /// <summary>
    /// Gets or sets the underlying <see cref="Bogus.Faker"/> instance used to generate random values for primitives, strings, dates, and other fundamental types.
    /// </summary>
    /// <remarks>
    /// This provides access to the Bogus library's random data generation capabilities. You can configure it directly to control locale, seed, and other random generation settings.
    /// </remarks>
    Faker Faker { get; set; }

    /// <summary>
    /// This forces the final initialization that happens right before <see cref="Generate(Type)"/>. It typically does not need to be called!<para/>
    /// It's available however when the underlying <see cref="AutoFakerBinder"/> or <see cref="CacheService"/> needs accessing.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Generates a single instance of the specified type with all properties and fields populated with fake data.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate. Must be a reference type (class).</typeparam>
    /// <returns>A new instance of <typeparamref name="TType"/> with all members populated with generated values.</returns>
    /// <remarks>
    /// This method automatically creates an instance and populates all public and private properties and fields with appropriate fake data.
    /// Nested objects are also generated recursively according to the configuration settings.
    /// </remarks>
    TType Generate<TType>();

    /// <summary>
    /// Generates a collection containing the specified number of instances of the specified type.
    /// </summary>
    /// <typeparam name="TType">The type of instances to generate. Must be a reference type (class).</typeparam>
    /// <param name="count">The number of instances to generate in the collection.</param>
    /// <returns>A <see cref="List{TType}"/> containing <paramref name="count"/> generated instances.</returns>
    /// <remarks>
    /// Each instance in the collection is independently generated with its own set of random values.
    /// </remarks>
    List<TType> Generate<TType>(int count);

    /// <summary>
    /// Generates an instance of a type that is not known at compile time.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to generate an instance for.</param>
    /// <returns>A new instance of the specified type with all members populated with generated values.</returns>
    /// <remarks>
    /// This method is useful when the type is determined at runtime. For compile-time known types, prefer the generic <see cref="Generate{TType}"/> method for better type safety.
    /// </remarks>
    object Generate(Type type);

    /// <summary>
    /// Configures all faker instances and generate requests.
    /// </summary>
    /// <param name="configure">A handler to build the default faker configuration.</param>
    void Configure(Action<IAutoFakerDefaultConfigBuilder> configure);

    /// <summary>
    /// Creates a seed locally scoped within the <seealso cref="Faker{T}"/> ignoring the globally scoped <seealso cref="Randomizer.Seed"/>.
    /// If this method is never called the global <seealso cref="Randomizer.Seed"/> is used.
    /// </summary>
    /// <param name="seed">The seed value to use within the <seealso cref="Faker{T}"/> instance.</param>
    void UseSeed(int seed);

    /// <summary>
    /// Sets a local time reference for all DateTime calculations used by
    /// the <seealso cref="Faker{T}"/> instance; unless refDate parameters are specified 
    /// with the corresponding Date.Methods().
    /// </summary>
    /// <param name="refDate">The anchored DateTime reference to use.</param>
    void UseDateTimeReference(DateTime? refDate);
}