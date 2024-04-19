using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class ReadOnlyCollectionGenerator<TType> : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        List<TType> items = context.GenerateMany<TType>();

        try
        {
            ReadOnlyCollection<TType> collection = new ReadOnlyCollection<TType>(items);
            return collection;
        }
        catch (Exception ex)
        {
            return null!;
        }
    }
}