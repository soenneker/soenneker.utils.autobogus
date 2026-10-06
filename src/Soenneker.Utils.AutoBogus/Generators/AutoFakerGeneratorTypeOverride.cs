using System;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class AutoFakerGeneratorTypeOverride<TType> : AutoFakerGeneratorOverride
{
    private readonly CachedType _cachedType;

    private Func<AutoFakerOverrideContext, TType> Generator { get; }

    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Runtime reflection requires members that trimming may remove. Use statically registered metadata instead.")]
    internal AutoFakerGeneratorTypeOverride(Func<AutoFakerOverrideContext, TType> generator)
    {
        Generator = generator ?? throw new ArgumentNullException(nameof(generator));

        _cachedType = StaticCacheService.Cache.GetCachedType(typeof(TType));
    }

    public override bool CanOverride(AutoFakerContext context)
    {
        return context.CachedType == _cachedType;
    }

    public override void Generate(AutoFakerOverrideContext context)
    {
        context.Instance = Generator.Invoke(context);
    }
}