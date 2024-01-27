using System;
using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class GeneratorTypeOverride<TType> : GeneratorOverride
{
    internal GeneratorTypeOverride(Func<AutoFakerContextOverride, TType> generator)
    {
        Type = typeof(TType);
        Generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    private Type Type { get; }

    private Func<AutoFakerContextOverride, TType> Generator { get; }

    public override bool CanOverride(AutoFakerContext context)
    {
        return context.GenerateType == Type;
    }

    public override void Generate(AutoFakerContextOverride context)
    {
        context.Instance = Generator.Invoke(context);
    }
}