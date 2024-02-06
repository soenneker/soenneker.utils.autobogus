using System;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class AutoFakerGeneratorTypeOverride<TType> : AutoFakerGeneratorOverride
{
    private CachedType CachedType { get; }

    private Func<AutoFakerOverrideContext, TType> Generator { get; }

    internal AutoFakerGeneratorTypeOverride(Func<AutoFakerOverrideContext, TType> generator)
    {
        CachedType = CacheService.Cache.GetCachedType(typeof(TType));

        Generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    public override bool CanOverride(AutoFakerContext context)
    {
        return context.CachedType == CachedType;
    }

    public override void Generate(AutoFakerOverrideContext context)
    {
        context.Instance = Generator.Invoke(context);
    }
}