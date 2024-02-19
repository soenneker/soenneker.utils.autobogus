using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Override.Abstract;

namespace Soenneker.Utils.AutoBogus.Override;

///<inheritdoc cref="IAutoFakerOverride"/>
public abstract class AutoFakerOverride<T> : AutoFakerGeneratorOverride, IAutoFakerOverride
{
    protected Type Type { get; }

    protected AutoFakerOverride()
    {
        Type = typeof(T);
    }

    /// <summary>
    /// Typically, SHOULD NOT be overriden again
    /// </summary>
    public override bool CanOverride(AutoFakerContext context)
    {
        bool shouldOverride = context.GenerateType == Type;

        return shouldOverride;
    }
}