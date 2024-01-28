using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

internal sealed class AutoFakerConfig
{
    internal const string DefaultLocale = "en";
    internal const int GenerateAttemptsThreshold = 3;

    internal static readonly Func<AutoFakerContext, int> DefaultRepeatCount = _ => 3;
    internal static readonly Func<AutoFakerContext, int> DefaultDataTableRowCount = _ => 15;
    internal static readonly Func<AutoFakerContext, int> DefaultRecursiveDepth = _ => 2;
    internal static readonly Func<AutoFakerContext, int?> DefaultTreeDepth = _ => null;
    internal static readonly Func<AutoFakerContext, DateTimeKind> DefaultDateTimeKind = _ => System.DateTimeKind.Local;

    internal string Locale { get; set; }
    internal Func<AutoFakerContext, DateTimeKind> DateTimeKind { get; set; }
    internal Func<AutoFakerContext, int> RepeatCount { get; set; }
    internal Func<AutoFakerContext, int> DataTableRowCount { get; set; }
    internal Func<AutoFakerContext, int> RecursiveDepth { get; set; }

    internal AutoFakerBinder FakerBinder { get; set; }

    internal List<Type> SkipTypes { get; set; }

    internal List<string> SkipPaths { get; set; }

    internal List<GeneratorOverride> Overrides { get; set; }

    public Func<AutoFakerContext, int?> TreeDepth { get; set; }

    public Faker? Faker { get; set; }

    internal AutoFakerConfig()
    {
        Locale = DefaultLocale;
        RepeatCount = DefaultRepeatCount;
        DataTableRowCount = DefaultDataTableRowCount;
        RecursiveDepth = DefaultRecursiveDepth;
        TreeDepth = DefaultTreeDepth;
        DateTimeKind = DefaultDateTimeKind;
        FakerBinder = new AutoFakerBinder();
        SkipTypes = [];
        SkipPaths = [];
        Overrides = [];

        Faker ??= new Faker(Locale);
    }

    internal AutoFakerConfig(AutoFakerConfig fakerConfig)
    {
        Locale = fakerConfig.Locale;
        RepeatCount = fakerConfig.RepeatCount;
        DataTableRowCount = fakerConfig.DataTableRowCount;
        RecursiveDepth = fakerConfig.RecursiveDepth;
        TreeDepth = fakerConfig.TreeDepth;
        DateTimeKind = fakerConfig.DateTimeKind;
        FakerBinder = fakerConfig.FakerBinder;
        SkipTypes = fakerConfig.SkipTypes;
        SkipPaths = fakerConfig.SkipPaths;
        Overrides = fakerConfig.Overrides;

        Faker = fakerConfig.Faker ?? new Faker(Locale);
    }
}