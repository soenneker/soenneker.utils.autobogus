using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Dtos;

internal class Proxy<T> : Proxy
{
    public override object Generate(AutoFakerContext context) => context.Generate<T>();
}