using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Dtos;

internal abstract class Proxy
{
    public abstract object Generate(AutoFakerContext context);
}