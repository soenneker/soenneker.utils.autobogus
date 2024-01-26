using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus;

///<inheritdoc cref="IAutoFaker"/>
public sealed class AutoFaker : IAutoFaker
{
    private AutoFaker(AutoFakerConfig fakerConfig)
    {
        FakerConfig = fakerConfig;
    }

    internal AutoFakerConfig FakerConfig { get; }

    TType IAutoFaker.Generate<TType>(Action<IAutoGenerateConfigBuilder> configure)
    {
        AutoFakerContext context = CreateContext(configure);
        return context.Generate<TType>();
    }

    List<TType> IAutoFaker.Generate<TType>(int count, Action<IAutoGenerateConfigBuilder> configure)
    {
        AutoFakerContext context = CreateContext(configure);
        return context.GenerateMany<TType>(count);
    }


    /// <summary>
    /// Configures all faker instances and generate requests.
    /// </summary>
    /// <param name="configure">A handler to build the default faker configuration.</param>
    public static void Configure(Action<IAutoFakerDefaultConfigBuilder>? configure)
    {
        if (configure == null)
            return;

        var builder = new AutoFakerConfigBuilder(DefaultConfigService.Config);
        configure.Invoke(builder);
    }

    /// <summary>
    /// Creates a configured <see cref="IAutoFaker"/> instance.
    /// </summary>
    /// <param name="configure">A handler to build the faker configuration.</param>
    /// <returns>The configured <see cref="IAutoFaker"/> instance.</returns>
    public static AutoFaker Create(Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        var config = new AutoFakerConfig(DefaultConfigService.Config);

        if (configure != null)
        {
            var builder = new AutoFakerConfigBuilder(config);

            configure.Invoke(builder);
        }

        return new AutoFaker(config);
    }

    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated instance.</returns>
    public static TType Generate<TType>(Action<IAutoGenerateConfigBuilder>? configure = null)
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
    public static List<TType> Generate<TType>(int count, Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        IAutoFaker faker = Create(configure);
        return faker.Generate<TType>(count);
    }

    private AutoFakerContext CreateContext(Action<IAutoGenerateConfigBuilder>? configure)
    {
        var config = new AutoFakerConfig(FakerConfig);

        if (configure != null)
        {
            var builder = new AutoFakerConfigBuilder(config);
            configure.Invoke(builder);
        }

        return new AutoFakerContext(config);
    }
}