using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class ExceptionGenerator : IAutoFakerGenerator
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        return new Exception(context.Faker.Lorem.Sentence());
    }
}