using System;
using Bogus;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Override;

public abstract class AutoFakerOverride<T> : AutoFakerGeneratorOverride
{
    protected Faker Faker { get; private set; }

    protected Type Type { get; }

    private readonly bool _fakerSet;

    protected AutoFakerOverride(Faker? faker = null)
    {
        Type = typeof(T);

        if (faker == null) 
            return;

        Faker = faker;
        _fakerSet = true;
    }

    /// <summary>
    /// Typically, SHOULD NOT be overriden again
    /// </summary>
    public override bool CanOverride(AutoFakerContext context)
    {
        bool shouldOverride = context.GenerateType == Type;

        if (shouldOverride && !_fakerSet)
            Faker = context.Faker;

        return shouldOverride;
    }
}