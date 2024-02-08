using System.Collections.Generic;
using Bogus;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Abstract;

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
    /// The underlying <see cref="Bogus.Faker"/> instance used to generate random values.
    /// </summary>
    public Faker Faker { get; }

    public IAutoFaker AutoFaker { get; }

    /// <summary>
    /// The requested rule sets provided for the generate request.
    /// </summary>
    public List<string>? RuleSets { get; }

    internal AutoFakerOverrideContext(AutoFakerContext generateContext)
    {
        GenerateType = generateContext.CachedType;
        GenerateName = generateContext.GenerateName;
        Faker = generateContext.Faker;
        AutoFaker = generateContext.AutoFaker;
        RuleSets = generateContext.RuleSets;
    }
}