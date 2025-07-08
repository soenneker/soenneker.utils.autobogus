using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class NullableGenerator<TType> : IAutoFakerGenerator where TType : struct
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        return context.Generate<TType>();
    }
}