using System;
using System.Collections.Generic;
using Soenneker.Reflection.Cache;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus;

/// <summary>
/// A class used to conveniently invoke generate requests.
/// </summary>
public sealed class AutoFaker : IAutoFaker
{
    internal static AutoConfig DefaultConfig = new AutoConfig();
    public static ReflectionCache Cache;

    private AutoFaker(AutoConfig config)
    {
        Config = config;
        Cache = new ReflectionCache();
    }

    internal AutoConfig Config { get; }

    TType IAutoFaker.Generate<TType>(Action<IAutoGenerateConfigBuilder> configure)
    {
        AutoGenerateContext context = CreateContext(configure);
        return context.Generate<TType>();
    }

    List<TType> IAutoFaker.Generate<TType>(int count, Action<IAutoGenerateConfigBuilder> configure)
    {
        AutoGenerateContext context = CreateContext(configure);
        return context.GenerateMany<TType>(count);
    }

    TType IAutoFaker.Generate<TType, TFaker>(Action<IAutoFakerConfigBuilder> configure)
    {
        AutoFaker<TType> faker = CreateFaker<TType, TFaker>(configure);
        return faker.Generate();
    }

    List<TType> IAutoFaker.Generate<TType, TFaker>(int count, Action<IAutoFakerConfigBuilder> configure)
    {
        AutoFaker<TType> faker = CreateFaker<TType, TFaker>(configure);
        return faker.Generate(count);
    }

    /// <summary>
    /// Configures all faker instances and generate requests.
    /// </summary>
    /// <param name="configure">A handler to build the default faker configuration.</param>
    public static void Configure(Action<IAutoFakerDefaultConfigBuilder> configure)
    {
        var builder = new AutoConfigBuilder(DefaultConfig);
        configure?.Invoke(builder);
    }

    /// <summary>
    /// Creates a configured <see cref="IAutoFaker"/> instance.
    /// </summary>
    /// <param name="configure">A handler to build the faker configuration.</param>
    /// <returns>The configured <see cref="IAutoFaker"/> instance.</returns>
    public static IAutoFaker Create(Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        var config = new AutoConfig(DefaultConfig);
        var builder = new AutoConfigBuilder(config);

        configure?.Invoke(builder);

        return new AutoFaker(config);
    }

    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated instance.</returns>
    public static TType Generate<TType>(Action<IAutoGenerateConfigBuilder> configure = null)
    {
        IAutoFaker faker = Create(configure);
        return faker.Generate<TType>();
    }

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="count">The number of instances to generate.</param>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated collection of instances.</returns>
    public static List<TType> Generate<TType>(int count, Action<IAutoGenerateConfigBuilder> configure = null)
    {
        IAutoFaker faker = Create(configure);
        return faker.Generate<TType>(count);
    }

    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/> based on the <typeparamref name="TFaker"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <typeparam name="TFaker">The type of faker instance to use.</typeparam>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated instance.</returns>
    public static TType Generate<TType, TFaker>(Action<IAutoFakerConfigBuilder> configure = null)
        where TType : class
        where TFaker : AutoFaker<TType>
    {
        IAutoFaker faker = Create();
        return faker.Generate<TType, TFaker>(configure);
    }

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/> based on the <typeparamref name="TFaker"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <typeparam name="TFaker">The type of faker instance to use.</typeparam>
    /// <param name="count">The number of instances to generate.</param>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated collection of instances.</returns>
    public static List<TType> Generate<TType, TFaker>(int count, Action<IAutoFakerConfigBuilder>? configure = null)
        where TType : class
        where TFaker : AutoFaker<TType>
    {
        IAutoFaker faker = Create();
        return faker.Generate<TType, TFaker>(count, configure);
    }

    private AutoGenerateContext CreateContext(Action<IAutoGenerateConfigBuilder>? configure)
    {
        var config = new AutoConfig(Config);
        var builder = new AutoConfigBuilder(config);

        configure?.Invoke(builder);

        return new AutoGenerateContext(config);
    }

    private AutoFaker<TType> CreateFaker<TType, TFaker>(Action<IAutoFakerConfigBuilder>? configure)
        where TType : class
        where TFaker : AutoFaker<TType>
    {
        // Invoke the config builder
        var config = new AutoConfig(Config);
        var builder = new AutoConfigBuilder(config);

        configure?.Invoke(builder);

        // Dynamically create the faker instance
        Type type = typeof(TFaker);
        var faker = (AutoFaker<TType>)Activator.CreateInstance(type, builder.Args);

        faker.Config = builder.Config;

        return faker;
    }
}