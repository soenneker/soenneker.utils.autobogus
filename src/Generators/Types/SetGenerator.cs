using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class SetGenerator<TType> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        ISet<TType> set;

        if (context.CachedType.IsInterface)
        {
            set = new HashSet<TType>();
        }
        else
        {
            try
            {
                set = (ISet<TType>)Activator.CreateInstance(context.CachedType.Type);
            }
            catch //(Exception e)
            {
                set = new HashSet<TType>();
            }
        }

        List<TType> items = context.GenerateMany<TType>();

        foreach (TType? item in items)
        {
            set.Add(item);
        }

        return set;
    }
}