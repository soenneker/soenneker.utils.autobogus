using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using Soenneker.Utils.AutoBogus.Services;
using Soenneker.Utils.AutoBogus.Utils;

namespace Soenneker.Utils.AutoBogus.Generators;

public static class AutoFakerGeneratorFactory
{
    internal static IAutoFakerGenerator GetGenerator(AutoFakerContext context)
    {
        IAutoFakerGenerator? cachedGenerator = context.Binder.GeneratorService.GetGenerator(context.CachedType);

        if (cachedGenerator != null)
            return cachedGenerator;

        IAutoFakerGenerator generator = CreateGenerator(context);

        List<AutoFakerGeneratorOverride>? overrides = null;

        if (context.Config.Overrides != null)
        {
            overrides = [];

            for (var i = 0; i < context.Config.Overrides.Count; i++)
            {
                AutoFakerGeneratorOverride o = context.Config.Overrides[i];

                if (o.CanOverride(context))
                    overrides.Add(o);
            }
        }

        if (overrides == null || overrides.Count == 0)
        {
            context.Binder.GeneratorService.SetGenerator(context.CachedType, generator);
            return generator;
        }

        var newOverrideGenerator = new AutoFakerGeneratorOverrideInvoker(generator, overrides);

        context.Binder.GeneratorService.SetGenerator(context.CachedType, newOverrideGenerator);
        return newOverrideGenerator;
    }

    internal static IAutoFakerGenerator CreateGenerator(AutoFakerContext context)
    {
        IAutoFakerGenerator? fundamentalGenerator = context.Binder.GeneratorService.GetFundamentalGenerator(context.CachedType);

        if (fundamentalGenerator != null)
            return fundamentalGenerator;

        CachedType? cachedType = context.CachedType;

        // Need check if the type is an in/out parameter and adjusted accordingly
        if (context.CachedType.IsByRef)
        {
            cachedType = cachedType.GetCachedElementType();
        }

        // Do some type -> generator mapping
        if (context.CachedType.IsArray)
        {
            cachedType = cachedType.GetCachedElementType();
            return CreateGenericGenerator(CachedTypeService.ArrayGenerator.Value, cachedType);
        }

        if (context.CachedType.IsEnum)
        {
            return CreateGenericGenerator(CachedTypeService.EnumGenerator.Value, cachedType);
        }

        if (context.CachedType.IsNullable)
        {
            cachedType = context.CachedType.GetCachedGenericArguments()![0];
            return CreateGenericGenerator(CachedTypeService.NullableGenerator.Value, cachedType);
        }

        // Check if an expando object needs to generator
        // This actually means an existing dictionary needs to be populated
        if (context.CachedType.IsExpandoObject)
        {
            return new ExpandoObjectGenerator();
        }

        (CachedType? collectionType, GenericCollectionType? genericCollectionType) = GenericTypeUtil.GetGenericCollectionType(context.CachedType);

        if (collectionType != null)
        {
            // For generic types we need to interrogate the inner types
            CachedType[] generics = collectionType.GetCachedGenericArguments()!;

            switch (genericCollectionType)
            {
                case GenericCollectionType.ReadOnlyDictionary:
                {
                    CachedType keyType = generics[0];
                    CachedType valueType = generics[1];

                    return CreateGenericGenerator(CachedTypeService.ReadOnlyDictionaryGenerator.Value, keyType, valueType);
                }
                case GenericCollectionType.ImmutableDictionary:
                case GenericCollectionType.Dictionary:
                case GenericCollectionType.SortedList:
                {
                    CachedType keyType = generics[0];
                    CachedType valueType = generics[1];

                    return CreateGenericGenerator(CachedTypeService.DictionaryGenerator.Value, keyType, valueType);
                }
                case GenericCollectionType.ReadOnlyList:
                case GenericCollectionType.ListType:
                case GenericCollectionType.ReadOnlyCollection:
                case GenericCollectionType.Collection:
                {
                    CachedType elementType = generics[0];
                    return CreateGenericGenerator(CachedTypeService.ListGenerator.Value, elementType);
                }
                case GenericCollectionType.Set:
                {
                    CachedType elementType = generics[0];
                    return CreateGenericGenerator(CachedTypeService.SetGenerator.Value, elementType);
                }
                case GenericCollectionType.Enumerable:
                {
                    if (collectionType == cachedType)
                    {
                        // Not a full list type, we can't fake it if it's anything other than
                        // the actual IEnumerable<T> interface itself.
                        CachedType elementType = generics[0];
                        return CreateGenericGenerator(CachedTypeService.EnumerableGenerator.Value, elementType);
                    }

                    break;
                }
            }
        }

        if (DataTableGenerator.TryCreateGenerator(context.CachedType, out DataTableGenerator dataTableGenerator))
        {
            return dataTableGenerator;
        }

        if (DataSetGenerator.TryCreateGenerator(context.CachedType, out DataSetGenerator dataSetGenerator))
        {
            return dataSetGenerator;
        }

        return CreateGenericGenerator(CachedTypeService.TypeGenerator.Value, cachedType);
    }

    private static IAutoFakerGenerator CreateGenericGenerator(CachedType cachedType, params CachedType[] genericTypes)
    {
        CachedType genericCachedType = cachedType.MakeCachedGenericType(genericTypes)!;

        var generator = (IAutoFakerGenerator) genericCachedType.CreateInstance();
        return generator;
    }
}