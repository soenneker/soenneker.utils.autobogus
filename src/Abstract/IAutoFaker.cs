using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Abstract;

/// <summary>
/// An interface for invoking generate requests.
/// </summary>
public interface IAutoFaker
{
    AutoFakerConfig Config { get; }

    AutoFakerBinder? Binder { get; set; }

    Faker Faker { get; set; }

    /// <summary>
    /// This forces the final initialization that happens right before <see cref="Generate(Type)"/>. It typically does not need to be called!<para/>
    /// It's available however when the underlying <see cref="AutoFakerBinder"/> or <see cref="CacheService"/> needs accessing.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <returns>The generated instance.</returns>
    TType Generate<TType>();

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="count">The number of instances to generate.</param>
    /// <returns>The generated collection of instances.</returns>
    List<TType> Generate<TType>(int count);

    /// <summary>
    /// Used to generate a type that is not known at compile time. Prefer <see cref="Generate{Type}"/> instead.
    /// </summary>
    object Generate(Type type);


    void Configure(Action<IAutoFakerDefaultConfigBuilder> configure);
}