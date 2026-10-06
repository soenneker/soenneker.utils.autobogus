using System.Runtime.InteropServices;
using System;
using System.Collections.Immutable;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types.Immutables;

// ReSharper disable once InconsistentNaming
/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class ImmutableArrayGenerator<TType> : IAutoFakerGenerator
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        try
        {
            TType[] items = context.GenerateArray<TType>();
            ImmutableArray<TType> array = ImmutableCollectionsMarshal.AsImmutableArray(items);
            return array;
        }
        catch (Exception)
        {
            return null!;
        }
    }
}