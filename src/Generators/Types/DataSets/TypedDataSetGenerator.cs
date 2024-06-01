using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Types.DataSets.Base;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataSets;

internal class TypedDataSetGenerator : BaseDataSetGenerator
{
    private readonly CachedType _dataSetType;

    public TypedDataSetGenerator(CachedType dataSetType)
    {
        _dataSetType = dataSetType;
    }

    public override object Generate(AutoFakerContext context)
    {
        var dataSet = _dataSetType.CreateInstance<DataSet>();

        List<DataTable> allTables = dataSet.Tables.OfType<DataTable>().ToList();
        var populatedTables = new HashSet<DataTable>();

        while (allTables.Count > 0)
        {
            var madeProgress = false;

            for (var i = 0; i < allTables.Count; i++)
            {
                DataTable table = allTables[i];

                IEnumerable<DataTable> referencedTables = table.Constraints
                    .OfType<ForeignKeyConstraint>()
                    .Select(constraint => constraint.RelatedTable);

                if (!referencedTables.Where(referencedTable => referencedTable != table).All(populatedTables.Contains))
                    continue;

                CachedType cachedType = context.CacheService.Cache.GetCachedType(table.GetType());

                if (!BaseDataTableGenerator.TryCreateGenerator(context,cachedType, out BaseDataTableGenerator? tableGenerator))
                    throw new Exception($"Couldn't create generator for typed table type {table.GetType()}");

                populatedTables.Add(table);

                context.Instance = table;

                BaseDataTableGenerator.PopulateRows(table, context);

                madeProgress = true;

                allTables.RemoveAt(i);
                i--;
            }

            if (!madeProgress)
                throw new Exception("Couldn't generate data for all tables in data set because there are constraints that can't be satisfied");
        }

        return dataSet;
    }
}