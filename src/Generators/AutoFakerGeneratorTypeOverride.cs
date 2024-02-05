using System;
using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class AutoFakerGeneratorTypeOverride<TType> : AutoFakerGeneratorOverride
{
    internal AutoFakerGeneratorTypeOverride(Func<AutoFakerOverrideContext, TType> generator)
    {
        Type = typeof(TType);
        Generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    private Type Type { get; }

    private Func<AutoFakerOverrideContext, TType> Generator { get; }

    public override bool CanOverride(AutoFakerContext context)
    {
        return context.GenerateType == Type;
    }

    public override void Generate(AutoFakerOverrideContext context)
    {
        context.Instance = Generator.Invoke(context);
    }
}