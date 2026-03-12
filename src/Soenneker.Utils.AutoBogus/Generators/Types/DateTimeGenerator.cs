using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

using System;

internal sealed class DateTimeGenerator
    : IAutoFakerGenerator
{
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