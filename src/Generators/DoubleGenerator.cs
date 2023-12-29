using Soenneker.Utils.AutoBogus.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class DoubleGenerator
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        return context.Faker.Random.Double();
    }
}