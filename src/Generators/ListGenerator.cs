using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class ListGenerator<TType>
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        IList<TType> list;

        try
        {
            list = (IList<TType>)Activator.CreateInstance(context.GenerateType);
        }
        catch
        {
            list = new List<TType>();
        }

        List<TType> items = context.GenerateMany<TType>();

        foreach (TType? item in items)
        {
            list.Add(item);
        }

        return list;
    }
}