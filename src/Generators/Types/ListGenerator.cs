using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class ListGenerator<TType> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        List<TType> list;

        if (context.CachedType.IsInterface)
        {
            list = [];
        }
        else
        {
            try
            {
                list = context.CachedType.CreateInstance<List<TType>>();
            }
            catch (Exception ex)
            {
                list = [];
            }
        }

        List<TType> items = context.GenerateMany<TType>();

        foreach (TType? item in items)
        {
            list.Add(item);
        }

        return list;
    }
}