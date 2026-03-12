using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

/// <summary>
/// This is the start of the journey of any top level type, and is used recursively
/// </summary>
/// <typeparam name="TType"></typeparam>
internal sealed class TypeGenerator<TType> : IAutoFakerGenerator
{
    object? IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        // Note that all instances are converted to object to cater for boxing and struct population
        // When setting a value via reflection on a struct a copy is made
        // This means the changes are applied to a different instance to the one created here
        CachedType cachedType = context.CacheService.Cache.GetCachedType(typeof(TType));

        // Protects against Generate<Action>, Generate<Func<>>, etc
        if (cachedType.IsDelegate)
            return null;

        object? instance = context.Binder.CreateInstanceWithRecursionGuard<TType>(context, cachedType);

        if (instance == null)
            return null;

        // Populate the generated instance
        context.Binder.PopulateInstance<TType>(instance, context, cachedType);

        return instance;
    }
}