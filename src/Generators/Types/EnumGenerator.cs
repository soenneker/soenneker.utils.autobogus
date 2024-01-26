using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class EnumGenerator<TType>
    : IAutoFakerGenerator
    where TType: struct, Enum
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        return context.Faker.Random.Enum<TType>();
    }
}