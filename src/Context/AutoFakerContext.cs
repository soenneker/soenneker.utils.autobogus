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
    /// Gets or sets the parent type that contains the member currently being generated.
    /// </summary>
    /// <remarks>
    /// This represents the type that owns the property or field being generated. For example, when generating a Person.Address property,
    /// the ParentType would be Person and the CachedType would be Address.
    /// </remarks>
    public CachedType? ParentType;

    /// <summary>
    /// Gets the <see cref="Type"/> instance for the type currently being generated.
    /// </summary>
    /// <remarks>
    /// This is a convenience property that extracts the Type from <see cref="CachedType"/>. May be null if CachedType is null,
    /// but remains non-nullable to avoid warnings in <see cref="AutoFakerOverride{T}"/> implementations.
    /// </remarks>
    public Type GenerateType => CachedType?.Type!;

    /// <summary>
    /// Gets or sets the cached type information for the type currently being generated.
    /// </summary>
    /// <remarks>
    /// This contains reflection information about the type being generated, including its properties, fields, constructors, etc.
    /// </remarks>
    public CachedType? CachedType { get; set; }

    /// <summary>
    /// Gets or sets the name of the member (property or field) currently being generated.
    /// </summary>
    /// <remarks>
    /// This is the name of the property or field that is being populated. For example, when generating a Person.Address property,
    /// this would be "Address".
    /// </remarks>
    public string? GenerateName;

    /// <summary>
    /// The underlying <see cref="Bogus.Faker"/> instance used to generate random values.
    /// </summary>
    public readonly Faker Faker;

    /// <summary>
    /// The configuration for the current generate request.
    /// </summary>
    public readonly AutoFakerConfig Config;

    /// <summary>
    /// The binder instance used for the current generate request.
    /// </summary>
    public readonly AutoFakerBinder Binder;

    /// <summary>
    /// The <see cref="AutoFaker"/> instance that created this context, if available.
    /// </summary>
    public readonly AutoFaker? AutoFaker;

    /// <summary>
    /// The cache service instance used for type caching.
    /// </summary>
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