using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class ListGenerator<TType> : IAutoFakerGenerator
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
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
            catch (Exception)
            {
                list = [];
            }
        }

        List<TType> items = context.GenerateMany<TType>();

        foreach (TType item in items)
        {
            list.Add(item);
        }

        return list;
    }
}