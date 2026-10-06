using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Dtos;

internal class Proxy<T> : Proxy
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
    public override object Generate(AutoFakerContext context) => context.Generate<T>();
}