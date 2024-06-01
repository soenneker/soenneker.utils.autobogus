using System;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Options;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

public sealed class AutoFakerConfig
{
    /// <summary>
    /// The Bogus.Faker locale to use when generating values.
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public string Locale;

    /// <summary>
    /// Registers the number of items to generate for a collection.
    /// </summary>
    public int RepeatCount;

    public int DataTableRowCount;
    public int RecursiveDepth;

    public HashSet<Type>? SkipTypes;

    public HashSet<string>? SkipPaths;

    public List<AutoFakerGeneratorOverride>? Overrides;

    public int? TreeDepth;

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public DateTimeKind DateTimeKind;

    public ReflectionCacheOptions? ReflectionCacheOptions;

    public AutoFakerConfig()
    {
        Locale = AutoFakerDefaultConfigOptions.Locale;
        RepeatCount = AutoFakerDefaultConfigOptions.RepeatCount;
        DataTableRowCount = AutoFakerDefaultConfigOptions.DataTableRowCount;
        RecursiveDepth = AutoFakerDefaultConfigOptions.RecursiveDepth;
        TreeDepth = AutoFakerDefaultConfigOptions.TreeDepth;
        DateTimeKind = AutoFakerDefaultConfigOptions.DateTimeKind;
    }
}