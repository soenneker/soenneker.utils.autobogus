using System;
using System.Linq;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Config.Base;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config;

internal sealed class AutoFakerConfigBuilder : IAutoFakerDefaultConfigBuilder, IAutoGenerateConfigBuilder, IAutoFakerConfigBuilder
{
    internal readonly AutoFakerConfig _autoFakerConfig;

    private readonly AutoFaker _autoFaker;

    internal AutoFakerConfigBuilder(AutoFakerConfig autoFakerConfig, AutoFaker autoFaker)
    {
        _autoFakerConfig = autoFakerConfig;
        _autoFaker = autoFaker;
    }

    internal object[]? Args { get; private set; }
  
    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithDataTableRowCount(int count) => WithDataTableRowCount<IAutoFakerDefaultConfigBuilder>(count, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithRecursiveDepth(int depth) => WithRecursiveDepth<IAutoFakerDefaultConfigBuilder>(depth, this);
    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithTreeDepth(int? depth) => WithTreeDepth<IAutoFakerDefaultConfigBuilder>(depth, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithBinder(AutoFakerBinder fakerBinder) => WithBinder<IAutoFakerDefaultConfigBuilder>(fakerBinder, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithSkip(Type type) => WithSkip<IAutoFakerDefaultConfigBuilder>(type, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithSkip(Type type, string memberName) =>
        WithSkip<IAutoFakerDefaultConfigBuilder>(type, memberName, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithSkip<TType>(string memberName) => WithSkip<IAutoFakerDefaultConfigBuilder, TType>(memberName, this);

    IAutoFakerDefaultConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerDefaultConfigBuilder>.WithOverride(AutoFakerGeneratorOverride autoFakerGeneratorOverride) =>
        WithOverride<IAutoFakerDefaultConfigBuilder>(autoFakerGeneratorOverride, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithDataTableRowCount(int count) => WithDataTableRowCount<IAutoGenerateConfigBuilder>(count, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithRecursiveDepth(int depth) => WithRecursiveDepth<IAutoGenerateConfigBuilder>(depth, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithTreeDepth(int? depth) => WithTreeDepth<IAutoGenerateConfigBuilder>(depth, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithBinder(AutoFakerBinder fakerBinder) => WithBinder<IAutoGenerateConfigBuilder>(fakerBinder, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithSkip(Type type) => WithSkip<IAutoGenerateConfigBuilder>(type, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithSkip(Type type, string memberName) => WithSkip<IAutoGenerateConfigBuilder>(type, memberName, this);
    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithSkip<TType>(string memberName) => WithSkip<IAutoGenerateConfigBuilder, TType>(memberName, this);

    IAutoGenerateConfigBuilder IBaseAutoFakerConfigBuilder<IAutoGenerateConfigBuilder>.WithOverride(AutoFakerGeneratorOverride autoFakerGeneratorOverride) =>
        WithOverride<IAutoGenerateConfigBuilder>(autoFakerGeneratorOverride, this);

    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithDataTableRowCount(int count) => WithDataTableRowCount<IAutoFakerConfigBuilder>(count, this);

    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithRecursiveDepth(int depth) => WithRecursiveDepth<IAutoFakerConfigBuilder>(depth, this);

    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithTreeDepth(int? depth) => WithTreeDepth<IAutoFakerConfigBuilder>(depth, this);

    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithBinder(AutoFakerBinder fakerBinder) => WithBinder<IAutoFakerConfigBuilder>(fakerBinder, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithSkip(Type type) => WithSkip<IAutoFakerConfigBuilder>(type, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithSkip(Type type, string memberName) => WithSkip<IAutoFakerConfigBuilder>(type, memberName, this);
    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithSkip<TType>(string memberName) => WithSkip<IAutoFakerConfigBuilder, TType>(memberName, this);

    IAutoFakerConfigBuilder IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>.WithOverride(AutoFakerGeneratorOverride autoFakerGeneratorOverride) =>
        WithOverride<IAutoFakerConfigBuilder>(autoFakerGeneratorOverride, this);

    IAutoFakerConfigBuilder IAutoFakerConfigBuilder.WithArgs(params object[] args) => WithArgs<IAutoFakerConfigBuilder>(args, this);

    internal TBuilder WithDataTableRowCount<TBuilder>(int count, TBuilder builder)
    {
        _autoFakerConfig.DataTableRowCount = count;

        return builder;
    }

    internal TBuilder WithRecursiveDepth<TBuilder>(int depth, TBuilder builder)
    {
        _autoFakerConfig.RecursiveDepth = depth;

        return builder;
    }

    internal TBuilder WithTreeDepth<TBuilder>(int? depth, TBuilder builder)
    {
        _autoFakerConfig.TreeDepth = depth;

        return builder;
    }

    private TBuilder WithBinder<TBuilder>(AutoFakerBinder fakerBinder, TBuilder builder)
    {
        _autoFaker.Binder = fakerBinder;

        return builder;
    }

    internal TBuilder WithSkip<TBuilder>(Type type, TBuilder builder)
    {
        _autoFakerConfig.SkipTypes ??= [];

        _autoFakerConfig.SkipTypes.Add(type);

        return builder;
    }

    internal TBuilder WithSkip<TBuilder>(Type type, string memberName, TBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(memberName))
            return builder;

        var path = $"{type.FullName}.{memberName}";

        _autoFakerConfig.SkipPaths ??= [];

        bool existing = _autoFakerConfig.SkipPaths.Any(s => s == path);

        if (!existing)
            _autoFakerConfig.SkipPaths.Add(path);

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

        _autoFakerConfig.Overrides ??= [];

        bool existing = _autoFakerConfig.Overrides.Any(o => o == autoFakerGeneratorOverride);

        if (!existing)
            _autoFakerConfig.Overrides.Add(autoFakerGeneratorOverride);

        return builder;
    }

    internal TBuilder WithArgs<TBuilder>(object[] args, TBuilder builder)
    {
        Args = args;
        return builder;
    }
}