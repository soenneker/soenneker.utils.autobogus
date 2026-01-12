using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Immutables;

// ReSharper disable once InconsistentNaming
internal sealed class ImmutableListGenerator<TType> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        try
        {
            TType[] items = context.GenerateArray<TType>();
            ImmutableList<TType> list = ImmutableList.CreateRange(items);
            return list;
        }
        catch (Exception)
        {
            return null!;
        }
    }
}