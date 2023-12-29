using Soenneker.Utils.AutoBogus.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class DecimalGenerator
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        return context.Faker.Random.Decimal();
    }
}