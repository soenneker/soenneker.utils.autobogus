using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Context;

/// <summary>
/// A class that provides context for a generate request.
/// </summary>
public sealed class AutoFakerContext
{
    /// <summary>
    /// The parent type of the type associated with the current generate request.
    /// </summary>
    public Type ParentType { get; set; }

    /// <summary>
    /// The type associated with the current generate request.
    /// </summary>
    public Type GenerateType { get; internal set; }

    /// <summary>
    /// The name associated with the current generate request.
    /// </summary>
    public string GenerateName { get; internal set; }

    /// <summary>
    /// The underlying <see cref="Bogus.Faker"/> instance used to generate random values.
    /// </summary>
    public Faker Faker { get; }

    /// <summary>
    /// The requested rule sets provided for the generate request.
    /// </summary>
    public List<string> RuleSets { get; internal set; }

    internal AutoFakerConfig FakerConfig { get; }
    internal Stack<Type> TypesStack { get; }

    internal object Instance { get; set; }

    internal IAutoFakerBinder FakerBinder => FakerConfig.FakerBinder;

    internal List<GeneratorOverride> Overrides => FakerConfig.Overrides;

    internal AutoFakerContext(AutoFakerConfig fakerConfig)
    {
        FakerConfig = fakerConfig;

        Faker = fakerConfig.Faker!;

        TypesStack = new Stack<Type>();

        RuleSets = [];
    }
}