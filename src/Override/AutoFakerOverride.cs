using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Override;

public abstract class AutoFakerOverride<T> : AutoFakerGeneratorOverride
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
        bool shouldOverride = context.CachedType.Type == Type;

        return shouldOverride;
    }
}