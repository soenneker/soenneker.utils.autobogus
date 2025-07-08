using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Override;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Context;

/// <summary>
/// A class that provides context for a generate request. A 'generate request' is a single call to `.Generate()` for example.
/// </summary>
public sealed class AutoFakerContext
{
    /// <summary>
    /// The parent type of the type associated with the current generate request.
    /// </summary>
    public CachedType? ParentType;

    /// <summary>
    /// Possible null reference if <see cref="CachedType"/> is null, but remaining as non-nullable for <see cref="AutoFakerOverride{T}"/> warnings.
    /// </summary>
    public Type GenerateType => CachedType?.Type!;

    /// <summary>
    /// The type associated with the current generate request.
    /// </summary>
    public CachedType? CachedType { get; set; }

    /// <summary>
    /// The name associated with the current generate request.
    /// </summary>
    public string? GenerateName;

    /// <summary>
    /// The underlying <see cref="Bogus.Faker"/> instance used to generate random values.
    /// </summary>
    public readonly Faker Faker;

    public readonly AutoFakerConfig Config;

    public readonly AutoFakerBinder Binder;

    public readonly AutoFaker? AutoFaker;

    public readonly CacheService CacheService;

    /// <summary>
    /// The requested rule sets provided for the generate request.
    /// </summary>
    public HashSet<string>? RuleSets;

    internal readonly Stack<int> TypesStack;

    internal readonly Stack<int> RecursiveConstructorStack;

    internal object? Instance;

    internal AutoFakerContext(AutoFaker autoFaker, CachedType? type = null)
        : this(autoFaker.Config, autoFaker.Binder, autoFaker.Faker, autoFaker.CacheService, type)
    {
        AutoFaker = autoFaker;
    }

    internal AutoFakerContext(AutoFakerConfig config, AutoFakerBinder binder, Faker faker, CacheService cacheService, CachedType? type = null)
    {
        Config = config;
        Binder = binder;
        Faker = faker;
        CacheService = cacheService;

        TypesStack = new Stack<int>();
        RecursiveConstructorStack = new Stack<int>();

        if (type == null)
            return;

        Setup(type);
    }

    internal void Setup(CachedType parentType, CachedType generateType, string name)
    {
        ParentType = parentType;
        GenerateName = name;
        CachedType = generateType;
    }

    internal void Setup(CachedType generateType)
    {
        CachedType = generateType;
    }
}