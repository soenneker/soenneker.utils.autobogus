using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class DateTimeOffsetGenerator : IAutoFakerGenerator
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        DateTimeOffset result = context.Faker.Date.RecentOffset();

        if (context.Config.DefaultTimezoneOffset.HasValue)
        {
            result = new DateTimeOffset(result.DateTime, context.Config.DefaultTimezoneOffset.Value);
        }

        return result;
    }
}