using System;
using Soenneker.Utils.AutoBogus.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class DateTimeOffsetGenerator
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        DateTime dateTime = context.Faker.Date.Recent();
        return new DateTimeOffset(dateTime);
    }
}