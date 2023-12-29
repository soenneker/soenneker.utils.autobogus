namespace Soenneker.Utils.AutoBogus.Abstract;

internal interface IAutoGenerator
{
    object Generate(AutoGenerateContext context);
}