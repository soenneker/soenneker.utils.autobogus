using Soenneker.Utils.AutoBogus.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class IPAddressGenerator
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        return context.Faker.Internet.IpAddress();
    }
}