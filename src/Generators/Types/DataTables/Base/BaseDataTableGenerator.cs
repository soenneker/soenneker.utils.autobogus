using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Dtos;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;

internal abstract class BaseDataTableGenerator : IAutoFakerGenerator
{
    private static readonly ConcurrentDictionary<Type, Proxy> _proxyCache = new();

    private static Proxy GetProxy(Type dataType)
    {
        return _proxyCache.GetOrAdd(dataType, static t =>
        {
            Type proxyType = typeof(Proxy<>).MakeGenericType(t);
            return (Proxy) Activator.CreateInstance(proxyType)!;
        });
    }

    public object Generate(AutoFakerContext context)
    {
        DataTable table = CreateTable(context);

        context.Instance = table;

        PopulateRows(table, context);

        return table;
    }

    public static bool IsTypedDataTableType(AutoFakerContext context, CachedType cachedType, out Type rowType)
    {
        rowType = default;

        while (cachedType.Type != null)
        {
            if (cachedType.IsGenericType && cachedType.GetGenericTypeDefinition() == typeof(TypedTableBase<>))
            {
                rowType = cachedType.GetGenericArguments()[0];
                return true;
            }

            if (cachedType.Type.BaseType == null)
                break;

            cachedType = context.CacheService.Cache.GetCachedType(cachedType.Type.BaseType);
        }

        return false;
    }

    public static bool TryCreateGenerator(AutoFakerContext context, CachedType tableType, out BaseDataTableGenerator generator)
    {
        generator = default;

        if (tableType.Type == typeof(DataTable))
            generator = new UntypedDataTableGenerator();
        else if (IsTypedDataTableType(context, tableType, out Type? rowType))
        {
            Type generatorType = typeof(TypedDataTableGenerator<,>).MakeGenericType(tableType.Type, rowType);

            generator = (BaseDataTableGenerator) Activator.CreateInstance(generatorType);
        }

        return generator != null;
    }

    public static void PopulateRows(DataTable table, AutoFakerContext context)
    {
        var rowCountIsSpecified = false;

        int rowCount = -1;

        if (context.Config.DataTableRowCount != null)
        {
            rowCountIsSpecified = true;
            rowCount = context.Config.DataTableRowCount;
        }

        if (rowCount < 0)
            rowCount = context.Faker.Random.Number(1, 20);

        var constrainedColumns = new Dictionary<DataColumn, ConstrainedColumnInfo>();
        var constraintHasUniqueColumns = new HashSet<ForeignKeyConstraint>();
        var referencedRowByConstraint = new Dictionary<ForeignKeyConstraint, DataRow>();
        var allConstraints = new List<ForeignKeyConstraint>();

        // Precompute unique columns once (avoids repeated UniqueConstraint enumeration)
        var uniqueColumns = new HashSet<DataColumn>();
        foreach (DataColumn col in table.Columns)
        {
            if (col.Unique)
                uniqueColumns.Add(col);
        }

        foreach (Constraint c in table.Constraints)
        {
            if (c is UniqueConstraint uc)
            {
                DataColumn[] cols = uc.Columns;
                for (var i = 0; i < cols.Length; i++)
                    uniqueColumns.Add(cols[i]);
            }
        }

        foreach (Constraint constraint in table.Constraints)
        {
            if (constraint is not ForeignKeyConstraint foreignKey)
                continue;

            allConstraints.Add(foreignKey);

            var containsUniqueColumns = false;
            DataColumn[] fkColumns = foreignKey.Columns;

            for (var i = 0; i < fkColumns.Length; i++)
            {
                if (uniqueColumns.Contains(fkColumns[i]))
                {
                    containsUniqueColumns = true;
                    break;
                }
            }

            for (var i = 0; i < foreignKey.Columns.Length; i++)
            {
                DataColumn column = foreignKey.Columns[i];
                DataColumn relatedColumn = foreignKey.RelatedColumns[i];

                if (!constrainedColumns.TryAdd(column,
                        new ConstrainedColumnInfo
                        {
                            Constraint = foreignKey,
                            RelatedColumn = relatedColumn,
                        }))
                    throw new Exception($"Column is constrained in multiple foreign key relationships simultaneously: {column.ColumnName} in DataTable {table.TableName}");
            }

            if (foreignKey.RelatedTable == table)
            {
                var hasNonNullableSelfReference = false;
                for (var i = 0; i < foreignKey.Columns.Length; i++)
                {
                    if (!foreignKey.Columns[i].AllowDBNull)
                    {
                        hasNonNullableSelfReference = true;
                        break;
                    }
                }

                if (hasNonNullableSelfReference)
                    throw new Exception($"Self-reference columns must be nullable so that at least one record can be added when the table is initially empty: DataTable {table.TableName}");
            }

            if (containsUniqueColumns)
                constraintHasUniqueColumns.Add(foreignKey);

            // Prepare a slot to be filled per-row.
            referencedRowByConstraint[foreignKey] = default;

            if (containsUniqueColumns
                && foreignKey.RelatedTable != table
                && foreignKey.RelatedTable.Rows.Count < rowCount)
            {
                if (rowCountIsSpecified)
                {
                    string remoteSubject = foreignKey.RelatedTable.TableName;

                    if (string.IsNullOrEmpty(remoteSubject))
                        remoteSubject = "another DataTable";

                    throw new ArgumentException(
                        $"Unable to satisfy the requested row count of {rowCount} because this table has a foreign key constraint on {remoteSubject} that must be unique, and that table only has {foreignKey.RelatedTable.Rows.Count} row(s).");
                }

                rowCount = foreignKey.RelatedTable.Rows.Count;
            }
        }

        while (rowCount > 0)
        {
            int rowIndex = table.Rows.Count;

            foreach (ForeignKeyConstraint? foreignKey in allConstraints)
            {
                referencedRowByConstraint[foreignKey] =
                    constraintHasUniqueColumns.Contains(foreignKey)
                        ? foreignKey.RelatedTable.Rows[rowIndex]
                        : foreignKey.RelatedTable.Rows.Count == 0
                            ? null
                            : foreignKey.RelatedTable.Rows[context.Faker.Random.Number(0, foreignKey.RelatedTable.Rows.Count - 1)];
            }

            var columnValues = new object[table.Columns.Count];

            for (var i = 0; i < table.Columns.Count; i++)
            {
                if (constrainedColumns.TryGetValue(table.Columns[i], out ConstrainedColumnInfo? constraintInfo))
                    columnValues[i] = referencedRowByConstraint[constraintInfo.Constraint]?[constraintInfo.RelatedColumn] ?? DBNull.Value;
                else
                    columnValues[i] = GenerateColumnValue(table.Columns[i], context);
            }

            table.Rows.Add(columnValues);

            rowCount--;
        }
    }

    private static object GenerateColumnValue(DataColumn dataColumn, AutoFakerContext context)
    {
        switch (Type.GetTypeCode(dataColumn.DataType))
        {
            case TypeCode.Empty:
            case TypeCode.DBNull: return null;
            case TypeCode.Boolean: return context.Faker.Random.Bool();
            case TypeCode.Char: return context.Faker.Lorem.Letter()[0];
            case TypeCode.SByte: return context.Faker.Random.SByte();
            case TypeCode.Byte: return context.Faker.Random.Byte();
            case TypeCode.Int16: return context.Faker.Random.Short();
            case TypeCode.UInt16: return context.Faker.Random.UShort();
            case TypeCode.Int32:
            {
                if (dataColumn.ColumnName.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                    return Interlocked.Increment(ref context.Faker.IndexFaker);
                return context.Faker.Random.Int();
            }
            case TypeCode.UInt32: return context.Faker.Random.UInt();
            case TypeCode.Int64: return context.Faker.Random.Long();
            case TypeCode.UInt64: return context.Faker.Random.ULong();
            case TypeCode.Single: return context.Faker.Random.Float();
            case TypeCode.Double: return context.Faker.Random.Double();
            case TypeCode.Decimal: return context.Faker.Random.Decimal();
            case TypeCode.DateTime: return context.Faker.Date.Between(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(+30));
            case TypeCode.String: return context.Faker.Lorem.Lines(1);

            default:
                if (dataColumn.DataType == typeof(TimeSpan))
                    return context.Faker.Date.Future() - context.Faker.Date.Future();
                if (dataColumn.DataType == typeof(Guid))
                    return context.Faker.Random.Guid();

                Proxy proxy = GetProxy(dataColumn.DataType);
                return proxy.Generate(context);
        }
    }

    protected abstract DataTable CreateTable(AutoFakerContext context);
}