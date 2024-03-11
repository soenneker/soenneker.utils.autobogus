using System.Data;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataSets.Base;

internal abstract class BaseDataSetGenerator : IAutoFakerGenerator
{
    public static bool TryCreateGenerator(CachedType dataSetType, out BaseDataSetGenerator? generator)
    {
        generator = default;
        
        CachedType cachedDataSetType = StaticCacheService.Cache.GetCachedType(typeof(DataSet));

        if (dataSetType.Type == cachedDataSetType.Type)
            generator = new UntypedDataSetGenerator();
        else if (cachedDataSetType.IsAssignableFrom(dataSetType.Type))
            generator = new TypedDataSetGenerator(dataSetType);

        return generator != null;
    }

    public abstract object Generate(AutoFakerContext context);
}