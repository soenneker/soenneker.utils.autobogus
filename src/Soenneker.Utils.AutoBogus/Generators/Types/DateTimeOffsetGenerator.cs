using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class DateTimeOffsetGenerator : IAutoFakerGenerator
{
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