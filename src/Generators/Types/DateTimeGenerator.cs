using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

using System;

internal sealed class DateTimeGenerator
    : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        return context.Config.DateTimeKind == DateTimeKind.Utc
            ? context.Faker.Date.Recent().ToUniversalTime()
            : context.Faker.Date.Recent();
    }
}