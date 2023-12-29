using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class EnumerableGenerator<TType>
    : IAutoGenerator
{
    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        return context.GenerateMany<TType>(); 
    }    
}