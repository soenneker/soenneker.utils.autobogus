using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal abstract class DataSetGenerator : IAutoFakerGenerator
{
    public static bool TryCreateGenerator(CachedType dataSetType, out DataSetGenerator? generator)
    {
        generator = default;

        CachedType cachedDataSetType = CacheService.Cache.GetCachedType(typeof(DataSet));

        if (dataSetType.Type == cachedDataSetType.Type)
            generator = new UntypedDataSetGenerator();
        else if (cachedDataSetType.IsAssignableFrom(dataSetType.Type))
            generator = new TypedDataSetGenerator(dataSetType);

        return generator != null;
    }

    public abstract object Generate(AutoFakerContext context);

    class UntypedDataSetGenerator : DataSetGenerator
    {
        public override object Generate(AutoFakerContext context)
        {
            var dataSet = new DataSet();

            CachedType cachedDataTableType = CacheService.Cache.GetCachedType(typeof(DataTable));

            if (!DataTableGenerator.TryCreateGenerator(cachedDataTableType, out DataTableGenerator? tableGenerator))
                throw new Exception("Internal error: Couldn't create generator for DataTable");

            for (int tableCount = context.Faker.Random.Int(2, 6); tableCount > 0; tableCount--)
                dataSet.Tables.Add((DataTable) tableGenerator.Generate(context));

            return dataSet;
        }
    }

    class TypedDataSetGenerator : DataSetGenerator
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
                bool madeProgress = false;

                for (int i = 0; i < allTables.Count; i++)
                {
                    DataTable table = allTables[i];

                    IEnumerable<DataTable> referencedTables = table.Constraints
                        .OfType<ForeignKeyConstraint>()
                        .Select(constraint => constraint.RelatedTable);

                    if (!referencedTables.Where(referencedTable => referencedTable != table).All(populatedTables.Contains))
                        continue;

                    CachedType cachedType = CacheService.Cache.GetCachedType(table.GetType());

                    if (!DataTableGenerator.TryCreateGenerator(cachedType, out DataTableGenerator? tableGenerator))
                        throw new Exception($"Couldn't create generator for typed table type {table.GetType()}");

                    populatedTables.Add(table);

                    context.Instance = table;

                    DataTableGenerator.PopulateRows(table, context);

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
}