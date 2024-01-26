using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Generators.Abstract;

internal interface IAutoFakerGenerator
{
    object Generate(AutoFakerContext context);
}