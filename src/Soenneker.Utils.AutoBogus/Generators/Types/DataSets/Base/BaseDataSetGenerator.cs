using System.Data;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types.DataSets.Base;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal abstract class BaseDataSetGenerator : IAutoFakerGenerator
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Runtime reflection requires members that trimming may remove. Use statically registered metadata instead.")]
    public static bool TryCreateGenerator(AutoFakerContext? context, CachedType dataSetType, out BaseDataSetGenerator? generator)
    {
        generator = null;
        
        CachedType cachedDataSetType = context.CacheService.Cache.GetCachedType(typeof(DataSet));

        if (dataSetType.Type == cachedDataSetType.Type)
            generator = new UntypedDataSetGenerator();
        else if (cachedDataSetType.IsAssignableFrom(dataSetType.Type))
            generator = new TypedDataSetGenerator(dataSetType);

        return generator != null;
    }

    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Automatic test data generation discovers arbitrary constructors and members at runtime.")]
    [global::System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Automatic test data generation constructs generic generators at runtime.")]
    public abstract object Generate(AutoFakerContext context);
}