using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Config;
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
    public Type? ParentType;

    /// <summary>
    /// The type associated with the current generate request.
    /// </summary>
    public Type? GenerateType;

    public CachedType CachedType;

    /// <summary>
    /// The name associated with the current generate request.
    /// </summary>
    public string GenerateName;

    /// <summary>
    /// The underlying <see cref="Bogus.Faker"/> instance used to generate random values.
    /// </summary>
    public readonly Faker Faker;

    public readonly AutoFakerConfig Config;

    public readonly AutoFakerBinder Binder;

    /// <summary>
    /// The requested rule sets provided for the generate request.
    /// </summary>
    public List<string> RuleSets;

    internal readonly Stack<int> TypesStack;

    internal object Instance;

    internal AutoFakerContext(AutoFakerConfig config, Faker faker, AutoFakerBinder binder, Type? type = null)
    {
        TypesStack = new Stack<int>();
        Config = config;

        Binder = binder;
        Faker = faker;

        RuleSets = [];

        if (type != null)
            Setup(type);
    }

    internal void Setup(Type parentType, CachedType generateType, string name)
    {
        ParentType = parentType;
        GenerateType = generateType.Type;
        GenerateName = name;
        CachedType = generateType;
    }

    internal void Setup(Type generateType)
    {
        GenerateType = generateType;
        CachedType = CacheService.Cache.GetCachedType(generateType);
    }
}