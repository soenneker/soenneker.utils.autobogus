using System;
using Soenneker.Utils.AutoBogus.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class EnumGenerator<TType>
    : IAutoGenerator
    where TType: struct, Enum
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        return context.Faker.Random.Enum<TType>();
    }
}