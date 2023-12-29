using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class ArrayGenerator<TType>
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        List<TType> items = context.GenerateMany<TType>();
        return items.ToArray();
    }
}