using System.Data;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataTables;

internal class TypedDataTableGenerator<TTable, TRow> : BaseDataTableGenerator
    where TTable : DataTable, new()
{
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic data table generation discovers columns at runtime.")]
    protected override DataTable CreateTable(AutoFakerContext context) => new TTable();
}