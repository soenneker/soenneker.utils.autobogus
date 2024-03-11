using System;
using System.Data;
using System.Linq;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataTables;

internal class UntypedDataTableGenerator : BaseDataTableGenerator
{
    protected override DataTable CreateTable(AutoFakerContext context)
    {
        var table = new DataTable();

        for (int i = context.Faker.Random.Int(3, 10); i >= 0; i--)
        {
            table.Columns.Add(
                new DataColumn
                {
                    ColumnName = context.Faker.Database.Column() + i,
                    DataType = Type.GetType("System." + context.Faker.PickRandom(
                        ((TypeCode[])Enum.GetValues(typeof(TypeCode)))
                        .Except(new[] { TypeCode.Empty, TypeCode.Object, TypeCode.DBNull })
                        .ToArray())),
                });
        }

        return table;
    }
}