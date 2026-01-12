using System;
using System.Collections.Generic;
using System.Reflection;
using Bogus;
using Soenneker.Reflection.Cache.Methods;
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

    public AutoFakerBinder? Binder { get; set; }

    public Faker Faker { get; set; }

    internal CacheService? CacheService { get; private set; }

    private CachedMethod? _generateMethodDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoFaker"/> class.
    /// </summary>
    /// <param name="autoFakerConfig">Optional configuration for the faker instance. If null, a default configuration is used.</param>
    public AutoFaker(AutoFakerConfig? autoFakerConfig = null)
    {
        Faker = new Faker();

        if (autoFakerConfig == null)
            Config = new AutoFakerConfig();
        else
            Config = autoFakerConfig;
    }

    public AutoFaker(Action<IAutoGenerateConfigBuilder>? configure)
    {
        Faker = new Faker();

        Config = new AutoFakerConfig();

        if (configure != null)
        {
            var builder = new AutoFakerConfigBuilder(Config);

            configure.Invoke(builder);
        }
    }

    public void Initialize()
    {
        CacheService ??= new CacheService(Config.ReflectionCacheOptions);
        Binder ??= new AutoFakerBinder();
    }

    public TType Generate<TType>()
    {
        Initialize();

        var context = new AutoFakerContext(this);
        return context.Generate<TType>()!;
    }

    public List<TType> Generate<TType>(int count)
    {
        Initialize();

        var context = new AutoFakerContext(this);
        return context.GenerateMany<TType>(count);
    }

    public object Generate(Type type)
    {
        Initialize();

        // Use Reflection.Cache's CachedMethod generic construction + compiled invokers (avoid MakeGenericMethod + reflection Invoke).
        _generateMethodDefinition ??= CacheService!.Cache.GetCachedType(typeof(AutoFaker)).GetCachedMethod("Generate");

        if (_generateMethodDefinition != null)
        {
            CachedMethod? constructed = _generateMethodDefinition.MakeCachedGenericMethod(type);
            if (constructed != null)
                return constructed.Invoke(this)!;
        }

        // Fallback: plain reflection
        MethodInfo method = typeof(AutoFaker).GetMethod("Generate", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)!;
        method = method.MakeGenericMethod(type);
        return method.Invoke(this, null)!;
    }

    public void Configure(Action<IAutoFakerDefaultConfigBuilder> configure)
    {
        var builder = new AutoFakerConfigBuilder(Config);
        configure.Invoke(builder);
    }

    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>. <para/>
    /// ⚠️ This creates a new Bogus.Faker on each call (expensive); use one AutoFaker across your context if possible.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated instance.</returns>
    public static TType GenerateStatic<TType>(Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        var faker = new AutoFaker(configure);
        return faker.Generate<TType>()!;
    }

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/>. <para/>
    /// ⚠️ This creates a new Bogus.Faker on each call (expensive); use one AutoFaker across your context if possible.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="count">The number of instances to generate.</param>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated collection of instances.</returns>
    public static List<TType> GenerateStatic<TType>(int count, Action<IAutoGenerateConfigBuilder>? configure = null)
    {
        var faker = new AutoFaker(configure);
        return faker.Generate<TType>(count);
    }

    public void UseSeed(int seed)
    {
        Faker.Random = new Randomizer(seed);
    }

    public void UseDateTimeReference(DateTime? refDate)
    {
        Faker.DateTimeReference = refDate;
    }
}