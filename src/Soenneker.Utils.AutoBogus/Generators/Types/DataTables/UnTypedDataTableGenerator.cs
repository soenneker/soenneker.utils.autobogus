using System;
using System.Data;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataTables;

internal class UntypedDataTableGenerator : BaseDataTableGenerator
{
    private static readonly TypeCode[] _supportedTypeCodes =
    [
        TypeCode.Boolean,
        TypeCode.Char,
        TypeCode.SByte,
        TypeCode.Byte,
        TypeCode.Int16,
        TypeCode.UInt16,
        TypeCode.Int32,
        TypeCode.UInt32,
        TypeCode.Int64,
        TypeCode.UInt64,
        TypeCode.Single,
        TypeCode.Double,
        TypeCode.Decimal,
        TypeCode.DateTime,
        TypeCode.String
    ];

    private static Type GetTypeFromTypeCode(TypeCode code) => code switch
    {
        TypeCode.Boolean => typeof(bool),
        TypeCode.Char => typeof(char),
        TypeCode.SByte => typeof(sbyte),
        TypeCode.Byte => typeof(byte),
        TypeCode.Int16 => typeof(short),
        TypeCode.UInt16 => typeof(ushort),
        TypeCode.Int32 => typeof(int),
        TypeCode.UInt32 => typeof(uint),
        TypeCode.Int64 => typeof(long),
        TypeCode.UInt64 => typeof(ulong),
        TypeCode.Single => typeof(float),
        TypeCode.Double => typeof(double),
        TypeCode.Decimal => typeof(decimal),
        TypeCode.DateTime => typeof(DateTime),
        TypeCode.String => typeof(string),
        _ => typeof(object)
    };

    protected override DataTable CreateTable(AutoFakerContext context)
    {
        var table = new DataTable();

        for (int i = context.Faker.Random.Int(3, 10); i >= 0; i--)
        {
            TypeCode code = context.Faker.PickRandom(_supportedTypeCodes);

            table.Columns.Add(
                new DataColumn
                {
                    ColumnName = context.Faker.Database.Column() + i,
                    DataType = GetTypeFromTypeCode(code),
                });
        }

        return table;
    }
}