using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

public sealed class AutoFakerConfig
{
    /// <summary>
    /// The Bogus.Faker locale to use when generating values.
    /// </summary>
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

    public DateTimeKind DateTimeKind;

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