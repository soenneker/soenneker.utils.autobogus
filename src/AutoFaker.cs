using System;
using System.Collections.Generic;
using System.Reflection;
using Bogus;
using Soenneker.Reflection.Cache.Types;
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
    public AutoFakerConfig Config { get; set; }

    internal AutoFakerBinder Binder { get; set; }

    public Faker Faker { get; set; }

    private readonly Lazy<MethodInfo> _nonTypeParameterMethod = new(() => {
        CachedType autoFakerType = CachedTypeService.AutoFaker.Value;

        MethodInfo methodInfo = autoFakerType.Type!.GetMethod("Generate", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)!;

        return methodInfo;
    });

    public AutoFaker(AutoFakerConfig? autoFakerConfig = null)
    {
        Faker = new Faker();

        if (autoFakerConfig == null)
            Config = new AutoFakerConfig();
        else
            Config = autoFakerConfig;

        Binder = new AutoFakerBinder(Config);
    }

    public AutoFaker(Action<IAutoGenerateConfigBuilder>? configure)
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

    public TType Generate<TType>()
    {
        var context = new AutoFakerContext(this);
        return context.Generate<TType>()!;
    }

    public List<TType> Generate<TType>(int count)
    {
        var context = new AutoFakerContext(this);
        return context.GenerateMany<TType>(count);
    }

    public object Generate(Type type)
    {
        // TODO: Optimize
        MethodInfo method = _nonTypeParameterMethod.Value.MakeGenericMethod(type);

        object? result = method.Invoke(this, null);
        return result!;
    }

    /// <summary>
    /// Configures all faker instances and generate requests.
    /// </summary>
    /// <param name="configure">A handler to build the default faker configuration.</param>
    public void Configure(Action<IAutoFakerDefaultConfigBuilder> configure)
    {
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
    public static TType GenerateStatic<TType>(Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        var faker = new AutoFaker(configure);
        return faker.Generate<TType>()!;
    }

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="count">The number of instances to generate.</param>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated collection of instances.</returns>
    [Obsolete("This creates a new Bogus.Faker on each call (expensive); use one AutoFaker across your context")]
    public static List<TType> GenerateStatic<TType>(int count, Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        var faker = new AutoFaker(configure);
        return faker.Generate<TType>(count);
    }
}