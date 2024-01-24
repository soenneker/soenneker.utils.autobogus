using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class ListGenerator<TType> : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        List<TType> list;

        try
        {
            list = (List<TType>)Activator.CreateInstance(context.GenerateType);
        }
        catch
        {
            list = [];
        }

        List<TType> items = context.GenerateMany<TType>();

        foreach (TType? item in items)
        {
            list.Add(item);
        }

        return list;
    }
}