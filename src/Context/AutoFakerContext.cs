using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Config;

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

    public Type? GenerateType => CachedType?.Type;

    /// <summary>
    /// The type associated with the current generate request.
    /// </summary>
    public CachedType? CachedType { get;  private set; }

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

    public readonly IAutoFaker AutoFaker;

    /// <summary>
    /// The requested rule sets provided for the generate request.
    /// </summary>
    public List<string>? RuleSets;

    internal readonly Stack<int> TypesStack;

    internal object? Instance;

    internal AutoFakerContext(AutoFaker autoFaker, CachedType? type = null)
    {
        AutoFaker = autoFaker;
        Config = autoFaker.Config;

        TypesStack = new Stack<int>();

        Binder = autoFaker.Binder;
        Faker = autoFaker.Faker;

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