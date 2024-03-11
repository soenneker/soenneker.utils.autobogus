using System.Collections.Generic;
using Bogus;
using Soenneker.Reflection.Cache.Types;

namespace Soenneker.Utils.AutoBogus.Context;

/// <summary>
/// A class that provides context when overriding a generate request.
/// </summary>
public sealed class AutoFakerOverrideContext
{
    /// <summary>
    /// The instance generated during the override.
    /// </summary>
    public object Instance { get; set; }

    /// <summary>
    /// The type associated with the current generate request.
    /// </summary>
    public CachedType GenerateType { get; }

    /// <summary>
    /// The name associated with the current generate request.
    /// </summary>
    public string GenerateName { get; }

    /// <summary>
    /// Will be null when using AutoFaker{T}, but don't want to warn on it.
    /// </summary>
    public AutoFaker AutoFaker { get; }

    /// <summary>
    /// The underlying <see cref="Bogus.Faker"/> instance used to generate random values.
    /// </summary>
    public Faker Faker { get; }

    /// <summary>
    /// The requested rule sets provided for the generate request.
    /// </summary>
    public List<string>? RuleSets { get; }

    internal AutoFakerOverrideContext(AutoFakerContext generateContext)
    {
        AutoFaker = generateContext.AutoFaker;
        GenerateType = generateContext.CachedType;
        GenerateName = generateContext.GenerateName;
        Faker = generateContext.Faker;
        RuleSets = generateContext.RuleSets;
    }
}