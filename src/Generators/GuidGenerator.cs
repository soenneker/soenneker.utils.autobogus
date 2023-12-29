using Soenneker.Utils.AutoBogus.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class GuidGenerator
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        return context.Faker.Random.Uuid();
    }
}