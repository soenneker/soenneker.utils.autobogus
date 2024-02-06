using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

public sealed class AutoFakerConfig
{
    internal string Locale;

    internal int RepeatCount;

    internal int DataTableRowCount;
    internal int RecursiveDepth;

    internal HashSet<Type>? SkipTypes;

    internal HashSet<string>? SkipPaths;

    internal List<AutoFakerGeneratorOverride>? Overrides;

    public int? TreeDepth;

    public DateTimeKind DateTimeKind;

    internal AutoFakerConfig()
    {
        Locale = AutoFakerDefaultConfigOptions.DefaultLocale;
        RepeatCount = AutoFakerDefaultConfigOptions.DefaultRepeatCount;
        DataTableRowCount = AutoFakerDefaultConfigOptions.DefaultDataTableRowCount;
        RecursiveDepth = AutoFakerDefaultConfigOptions.DefaultRecursiveDepth;
        TreeDepth = AutoFakerDefaultConfigOptions.DefaultTreeDepth;
        DateTimeKind = AutoFakerDefaultConfigOptions.DefaultDateTimeKind;
    }
}