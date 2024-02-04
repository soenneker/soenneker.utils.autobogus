using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus;

///<inheritdoc cref="IAutoFaker"/>
public sealed class AutoFaker : IAutoFaker
{
    public AutoFakerConfig Config { get; private set; }

    internal AutoFakerBinder Binder { get; set; }

    public Faker Faker { get; set; }

    public AutoFaker(Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        Faker = new Faker();

        Config = new AutoFakerConfig();

        if (configure != null)
        {
            var builder = new AutoFakerConfigBuilder(Config, this);

            configure.Invoke(builder);
        }

        Binder = new AutoFakerBinder(Config);
    }

    public void SetConfig(AutoFakerConfig config)
    {
        Config = config;
    }

    public void SetFaker(Faker faker)
    {
        Faker = faker;
    }

    TType IAutoFaker.Generate<TType>(Action<IAutoGenerateConfigBuilder>? configure)
    {
        AutoFakerContext context = CreateContext(configure);
        return context.Generate<TType>();
    }

    List<TType> IAutoFaker.Generate<TType>(int count, Action<IAutoGenerateConfigBuilder>? configure)
    {
        AutoFakerContext context = CreateContext(configure);
        return context.GenerateMany<TType>(count);
    }

    /// <summary>
    /// Configures all faker instances and generate requests.
    /// </summary>
    /// <param name="configure">A handler to build the default faker configuration.</param>
    public void Configure(Action<IAutoFakerDefaultConfigBuilder>? configure)
    {
        if (configure == null)
            return;

        var builder = new AutoFakerConfigBuilder(Config, this);
        configure.Invoke(builder);
    }

    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated instance.</returns>
    [Obsolete("This creates a new Bogus.Faker on each call (expensive); use one AutoFaker across your context")]
    public static TType Generate<TType>(Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        IAutoFaker faker = new AutoFaker(configure);
        return faker.Generate<TType>();
    }

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="count">The number of instances to generate.</param>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated collection of instances.</returns>
    [Obsolete("This creates a new Bogus.Faker on each call (expensive); use one AutoFaker across your context")]
    public static List<TType> Generate<TType>(int count, Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        IAutoFaker faker = new AutoFaker(configure);
        return faker.Generate<TType>(count);
    }

    private AutoFakerContext CreateContext(Action<IAutoGenerateConfigBuilder>? configure)
    {
        if (configure == null)
            return new AutoFakerContext(Config, Faker, Binder);

        var builder = new AutoFakerConfigBuilder(Config, this);
        configure.Invoke(builder);

        return new AutoFakerContext(Config, Faker, Binder);
    }
}