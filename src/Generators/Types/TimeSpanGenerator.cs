using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using System;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class TimeSpanGenerator: IAutoFakerGenerator
{
    private const long _minTicks = 60000000;        // 1 minute in ticks
    private const long _maxTicks = 36000000000;     // 1 hour in ticks

    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        TimeSpan timeSpan = TimeSpan.FromTicks(context.Faker.Random.Long(_minTicks, _maxTicks));
        return timeSpan;
    }
}