using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class CollectionGenerator<TType> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        Collection<TType> collection;

        if (context.CachedType.IsInterface)
        {
            collection = [];
        }
        else
        {
            try
            {
                collection = context.CachedType.CreateInstance<Collection<TType>>();
            }
            catch (Exception ex)
            {
                collection = [];
            }
        }

        List<TType> items = context.GenerateMany<TType>();

        for (var i = 0; i < items.Count; i++)
        {
            TType item = items[i];
            collection.Add(item);
        }

        return collection;
    }
}