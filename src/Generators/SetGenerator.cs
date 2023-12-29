using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class SetGenerator<TType>
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        ISet<TType> set;

        try
        {
            set = (ISet<TType>)Activator.CreateInstance(context.GenerateType);
        }
        catch
        {
            set = new HashSet<TType>();
        }

        List<TType> items = context.GenerateMany<TType>();

        foreach (TType? item in items)
        {
            set.Add(item);
        }

        return set;
    }
}