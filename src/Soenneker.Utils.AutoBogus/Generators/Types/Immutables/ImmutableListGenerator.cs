using System;
using System.Collections.Immutable;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Immutables;

// ReSharper disable once InconsistentNaming
/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class ImmutableListGenerator<TType> : IAutoFakerGenerator
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
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