using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

using System;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class DateTimeGenerator
    : IAutoFakerGenerator
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        DateTime dateTime = context.Faker.Date.Recent();

        if (context.Config.DateTimeKind == DateTimeKind.Utc)
        {
            return dateTime.ToUniversalTime();
        }

        return dateTime;
    }
}