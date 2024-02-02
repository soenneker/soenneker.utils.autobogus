using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

internal sealed class AutoFakerConfig
{
    internal const string DefaultLocale = "en";
    internal const int GenerateAttemptsThreshold = 3;

    internal const int DefaultRepeatCount = 3;
    internal const int DefaultDataTableRowCount = 15;

    internal const int DefaultRecursiveDepth = 2;

    internal static readonly int? DefaultTreeDepth = null;

    internal string Locale { get; set; }
    internal int RepeatCount { get; set; }
    internal int DataTableRowCount { get; set; }
    internal int RecursiveDepth { get; set; }

    internal HashSet<Type>? SkipTypes { get; set; }

    internal HashSet<string>? SkipPaths { get; set; }

    internal List<AutoFakerGeneratorOverride>? Overrides { get; set; }

    public int? TreeDepth { get; set; }

    public Faker? Faker { get; set; }

    internal AutoFakerConfig()
    {
        Locale = DefaultLocale;
        RepeatCount = DefaultRepeatCount;
        DataTableRowCount = DefaultDataTableRowCount;
        RecursiveDepth = DefaultRecursiveDepth;
        TreeDepth = DefaultTreeDepth;

        if (Faker != null)
            return;

        Faker = new Faker(Locale);
    }

    internal AutoFakerConfig(AutoFakerConfig fakerConfig)
    {
        Locale = fakerConfig.Locale;
        RepeatCount = fakerConfig.RepeatCount;
        DataTableRowCount = fakerConfig.DataTableRowCount;
        RecursiveDepth = fakerConfig.RecursiveDepth;
        TreeDepth = fakerConfig.TreeDepth;

        SkipTypes = fakerConfig.SkipTypes;
        SkipPaths = fakerConfig.SkipPaths;
        Overrides = fakerConfig.Overrides;

        if (Faker != null)
            return;

        Faker = fakerConfig.Faker ?? new Faker(Locale);
    }
}