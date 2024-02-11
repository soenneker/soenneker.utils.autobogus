using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

public sealed class AutoFakerConfig
{
    public string Locale;

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
        Locale = AutoFakerDefaultConfigOptions.DefaultLocale;
        RepeatCount = AutoFakerDefaultConfigOptions.DefaultRepeatCount;
        DataTableRowCount = AutoFakerDefaultConfigOptions.DefaultDataTableRowCount;
        RecursiveDepth = AutoFakerDefaultConfigOptions.DefaultRecursiveDepth;
        TreeDepth = AutoFakerDefaultConfigOptions.DefaultTreeDepth;
        DateTimeKind = AutoFakerDefaultConfigOptions.DefaultDateTimeKind;
    }
}