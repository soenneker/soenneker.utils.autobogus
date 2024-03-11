using System;
using System.Data;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Types.DataSets.Base;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataSets;

internal class UntypedDataSetGenerator : BaseDataSetGenerator
{
    public override object Generate(AutoFakerContext context)
    {
        var dataSet = new DataSet();

        CachedType cachedDataTableType = context.CacheService.Cache.GetCachedType(typeof(DataTable));

        if (!BaseDataTableGenerator.TryCreateGenerator(cachedDataTableType, out BaseDataTableGenerator? tableGenerator))
            throw new Exception("Internal error: Couldn't create generator for DataTable");

        for (int tableCount = context.Faker.Random.Int(2, 6); tableCount > 0; tableCount--)
            dataSet.Tables.Add((DataTable)tableGenerator.Generate(context));

        return dataSet;
    }
}