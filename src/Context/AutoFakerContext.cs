using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Generators;
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
    public Type? ParentType { get; private set; }

    /// <summary>
    /// The type associated with the current generate request.
    /// </summary>
    public Type? GenerateType { get; private set; }

    public CachedType CachedType { get; private set; }

    /// <summary>
    /// The name associated with the current generate request.
    /// </summary>
    public string GenerateName { get; private set; }

    /// <summary>
    /// The underlying <see cref="Bogus.Faker"/> instance used to generate random values.
    /// </summary>
    public Faker Faker { get; }

    /// <summary>
    /// The requested rule sets provided for the generate request.
    /// </summary>
    public List<string> RuleSets { get; internal set; }

    internal AutoFakerConfig AutoFakerConfig { get; }

    internal Stack<Type> TypesStack { get; }

    internal object Instance { get; set; }

    internal List<AutoFakerGeneratorOverride>? Overrides => AutoFakerConfig.Overrides;

    internal AutoFakerContext(AutoFakerConfig autoFakerConfig, Type? type = null)
    {
        AutoFakerConfig = autoFakerConfig;

        Faker = autoFakerConfig.Faker!;

        TypesStack = new Stack<Type>();

        RuleSets = [];

        if (type != null)
            Setup(type);
    }

    internal void Setup(Type parentType, Type generateType, string name)
    {
        ParentType = parentType;
        GenerateType = generateType;
        GenerateName = name;
        CachedType = CacheService.Cache.GetCachedType(generateType);
    }

    internal void Setup(Type generateType)
    {
        GenerateType = generateType;
        CachedType = CacheService.Cache.GetCachedType(generateType);
    }
}