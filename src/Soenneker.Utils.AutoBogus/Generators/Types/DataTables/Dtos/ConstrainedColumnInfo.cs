using System.Data;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Dtos;

internal class ConstrainedColumnInfo
{
    public ForeignKeyConstraint Constraint;

    public DataColumn RelatedColumn;
}