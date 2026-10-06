using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class ReadOnlyCollectionGenerator<TType> : IAutoFakerGenerator
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        List<TType> items = context.GenerateMany<TType>();

        try
        {
            var collection = new ReadOnlyCollection<TType>(items);
            return collection;
        }
        catch (Exception)
        {
            return null!;
        }
    }
}