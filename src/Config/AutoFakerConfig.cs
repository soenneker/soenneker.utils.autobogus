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

    public TimeSpan? DefaultTimezoneOffset;

    public ReflectionCacheOptions? ReflectionCacheOptions;

    /// <summary>
    /// Indicates whether generation should be performed using shallow copying instead of deep copying.
    /// </summary>
    /// <remarks>When set to <see langword="true"/>, only the top-level structure is generated, and nested
    /// objects or collections are not recursively copied. Use this option when a full deep copy is unnecessary or to
    /// improve performance for large object graphs.</remarks>
    public bool ShallowGenerate;

    public AutoFakerConfig()
    {
        Locale = AutoFakerDefaultConfigOptions.Locale;
        RepeatCount = AutoFakerDefaultConfigOptions.RepeatCount;
        DataTableRowCount = AutoFakerDefaultConfigOptions.DataTableRowCount;
        RecursiveDepth = AutoFakerDefaultConfigOptions.RecursiveDepth;
        TreeDepth = AutoFakerDefaultConfigOptions.TreeDepth;
        DateTimeKind = AutoFakerDefaultConfigOptions.DateTimeKind;
        DefaultTimezoneOffset = AutoFakerDefaultConfigOptions.DefaultTimezoneOffset;
        ShallowGenerate = AutoFakerDefaultConfigOptions.ShallowGenerate;
    }
}