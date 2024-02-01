using System;
using System.Linq;
using Bogus;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Config.Base;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

internal sealed class AutoFakerConfigBuilder : IAutoFakerDefaultConfigBuilder, IAutoGenerateConfigBuilder, IAutoFakerConfigBuilder
{
    internal AutoFakerConfigBuilder(AutoFakerConfig fakerConfig)
    {
        FakerConfig = fakerConfig;
    }

    internal AutoFakerConfig FakerConfig { get; }

    internal object[] Args { get; private set; }

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithLocale(string locale) => WithLocale<IAutoFakerDefaultConfigBuilder>(locale, this);
    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithRepeatCount(int count) => WithRepeatCount<IAutoFakerDefaultConfigBuilder>(context => count, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithRepeatCount(Func<AutoFakerContext, int> count) =>
        WithRepeatCount<IAutoFakerDefaultConfigBuilder>(count, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithDataTableRowCount(int count) => WithDataTableRowCount<IAutoFakerDefaultConfigBuilder>(context => count, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithDataTableRowCount(Func<AutoFakerContext, int> count) =>
        WithDataTableRowCount<IAutoFakerDefaultConfigBuilder>(count, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithRecursiveDepth(int depth) => WithRecursiveDepth<IAutoFakerDefaultConfigBuilder>(context => depth, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithRecursiveDepth(Func<AutoFakerContext, int> depth) =>
        WithRecursiveDepth<IAutoFakerDefaultConfigBuilder>(depth, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithTreeDepth(int? depth) => WithTreeDepth<IAutoFakerDefaultConfigBuilder>(context => depth, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithTreeDepth(Func<AutoFakerContext, int?> depth) =>
        WithTreeDepth<IAutoFakerDefaultConfigBuilder>(depth, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithBinder(AutoFakerBinder fakerBinder) => WithBinder<IAutoFakerDefaultConfigBuilder>(fakerBinder, this);
    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithFaker(Faker faker) => WithFaker<IAutoFakerDefaultConfigBuilder>(faker, this);
    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithSkip(Type type) => WithSkip<IAutoFakerDefaultConfigBuilder>(type, this);
    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithSkip(Type type, string memberName) => WithSkip<IAutoFakerDefaultConfigBuilder>(type, memberName, this);
    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithSkip<TType>(string memberName) => WithSkip<IAutoFakerDefaultConfigBuilder, TType>(memberName, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithOverride(AutoFakerGeneratorOverride autoFakerGeneratorOverride) =>
        WithOverride<IAutoFakerDefaultConfigBuilder>(autoFakerGeneratorOverride, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithLocale(string locale) => WithLocale<IAutoGenerateConfigBuilder>(locale, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithRepeatCount(int count) => WithRepeatCount<IAutoGenerateConfigBuilder>(context => count, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithRepeatCount(Func<AutoFakerContext, int> count) => WithRepeatCount<IAutoGenerateConfigBuilder>(count, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithDataTableRowCount(int count) => WithDataTableRowCount<IAutoGenerateConfigBuilder>(context => count, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithDataTableRowCount(Func<AutoFakerContext, int> count) =>
        WithDataTableRowCount<IAutoGenerateConfigBuilder>(count, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithRecursiveDepth(int depth) => WithRecursiveDepth<IAutoGenerateConfigBuilder>(context => depth, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithRecursiveDepth(Func<AutoFakerContext, int> depth) => WithRecursiveDepth<IAutoGenerateConfigBuilder>(depth, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithTreeDepth(int? depth) => WithTreeDepth<IAutoGenerateConfigBuilder>(context => depth, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithTreeDepth(Func<AutoFakerContext, int?> depth) => WithTreeDepth<IAutoGenerateConfigBuilder>(depth, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithBinder(AutoFakerBinder fakerBinder) => WithBinder<IAutoGenerateConfigBuilder>(fakerBinder, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithFaker(Faker faker) => WithFaker<IAutoGenerateConfigBuilder>(faker, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithSkip(Type type) => WithSkip<IAutoGenerateConfigBuilder>(type, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithSkip(Type type, string memberName) => WithSkip<IAutoGenerateConfigBuilder>(type, memberName, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithSkip<TType>(string memberName) => WithSkip<IAutoGenerateConfigBuilder, TType>(memberName, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithOverride(AutoFakerGeneratorOverride autoFakerGeneratorOverride) =>
        WithOverride<IAutoGenerateConfigBuilder>(autoFakerGeneratorOverride, this);

    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithLocale(string locale) => WithLocale<IAutoFakerConfigBuilder>(locale, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithRepeatCount(int count) => WithRepeatCount<IAutoFakerConfigBuilder>(context => count, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithRepeatCount(Func<AutoFakerContext, int> count) => WithRepeatCount<IAutoFakerConfigBuilder>(count, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithDataTableRowCount(int count) => WithDataTableRowCount<IAutoFakerConfigBuilder>(context => count, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithDataTableRowCount(Func<AutoFakerContext, int> count) => WithDataTableRowCount<IAutoFakerConfigBuilder>(count, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithRecursiveDepth(int depth) => WithRecursiveDepth<IAutoFakerConfigBuilder>(context => depth, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithRecursiveDepth(Func<AutoFakerContext, int> depth) => WithRecursiveDepth<IAutoFakerConfigBuilder>(depth, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithTreeDepth(int? depth) => WithTreeDepth<IAutoFakerConfigBuilder>(context => depth, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithTreeDepth(Func<AutoFakerContext, int?> depth) => WithTreeDepth<IAutoFakerConfigBuilder>(depth, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithBinder(AutoFakerBinder fakerBinder) => WithBinder<IAutoFakerConfigBuilder>(fakerBinder, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithFaker(Faker faker) => WithFaker<IAutoFakerConfigBuilder>(faker, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithSkip(Type type) => WithSkip<IAutoFakerConfigBuilder>(type, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithSkip(Type type, string memberName) => WithSkip<IAutoFakerConfigBuilder>(type, memberName, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithSkip<TType>(string memberName) => WithSkip<IAutoFakerConfigBuilder, TType>(memberName, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithOverride(AutoFakerGeneratorOverride autoFakerGeneratorOverride) => WithOverride<IAutoFakerConfigBuilder>(autoFakerGeneratorOverride, this);
    IAutoFakerConfigBuilder IAutoFakerConfigBuilder.WithArgs(params object[] args) => WithArgs<IAutoFakerConfigBuilder>(args, this);

    internal TBuilder WithLocale<TBuilder>(string? locale, TBuilder builder)
    {
        FakerConfig.Locale = locale ?? AutoFakerConfig.DefaultLocale;
        return builder;
    }

    internal TBuilder WithRepeatCount<TBuilder>(Func<AutoFakerContext, int>? count, TBuilder builder)
    {
        FakerConfig.RepeatCount = count ?? AutoFakerConfig.DefaultRepeatCount;
        return builder;
    }

    internal TBuilder WithDataTableRowCount<TBuilder>(Func<AutoFakerContext, int>? count, TBuilder builder)
    {
        FakerConfig.DataTableRowCount = count ?? AutoFakerConfig.DefaultDataTableRowCount;
        return builder;
    }

    internal TBuilder WithRecursiveDepth<TBuilder>(Func<AutoFakerContext, int>? depth, TBuilder builder)
    {
        FakerConfig.RecursiveDepth = depth ?? AutoFakerConfig.DefaultRecursiveDepth;
        return builder;
    }

    internal TBuilder WithTreeDepth<TBuilder>(Func<AutoFakerContext, int?>? depth, TBuilder builder)
    {
        FakerConfig.TreeDepth = depth ?? AutoFakerConfig.DefaultTreeDepth;
        return builder;
    }

    private TBuilder WithBinder<TBuilder>(AutoFakerBinder? fakerBinder, TBuilder builder)
    {
        FakerConfig.FakerBinder = fakerBinder ?? new AutoFakerBinder();
        return builder;
    }

    internal TBuilder WithFaker<TBuilder>(Faker faker, TBuilder builder)
    {
        FakerConfig.Faker = faker;
        return builder;
    }

    internal TBuilder WithSkip<TBuilder>(Type type, TBuilder builder)
    {
        bool existing = FakerConfig.SkipTypes.Any(t => t == type);

        if (!existing)
            FakerConfig.SkipTypes.Add(type);

        return builder;
    }

    internal TBuilder WithSkip<TBuilder>(Type type, string memberName, TBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(memberName))
            return builder;

        var path = $"{type.FullName}.{memberName}";
        bool existing = FakerConfig.SkipPaths.Any(s => s == path);

        if (!existing)
            FakerConfig.SkipPaths.Add(path);

        return builder;
    }

    internal TBuilder WithSkip<TBuilder, TType>(string memberName, TBuilder builder)
    {
        return WithSkip(typeof(TType), memberName, builder);
    }

    internal TBuilder WithOverride<TBuilder>(AutoFakerGeneratorOverride? autoFakerGeneratorOverride, TBuilder builder)
    {
        if (autoFakerGeneratorOverride == null)
            return builder;

        bool existing = FakerConfig.Overrides.Any(o => o == autoFakerGeneratorOverride);

        if (!existing)
        {
            FakerConfig.Overrides.Add(autoFakerGeneratorOverride);
        }

        return builder;
    }

    internal TBuilder WithArgs<TBuilder>(object[] args, TBuilder builder)
    {
        Args = args;
        return builder;
    }
}