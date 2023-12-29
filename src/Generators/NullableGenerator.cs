using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class NullableGenerator<TType>
    : IAutoGenerator
    where TType : struct
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        return context.Generate<TType>();
    }
}