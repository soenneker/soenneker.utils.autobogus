using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class ShortGenerator: IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        return context.Faker.Random.Short();
    }
}