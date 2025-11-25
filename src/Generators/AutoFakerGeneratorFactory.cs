using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using Soenneker.Utils.AutoBogus.Generators.Types.DataSets.Base;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;
using Soenneker.Utils.AutoBogus.Services;
using Soenneker.Utils.AutoBogus.Utils;

namespace Soenneker.Utils.AutoBogus.Generators;

public static class AutoFakerGeneratorFactory
{
    internal static IAutoFakerGenerator GetGenerator(AutoFakerContext context)
    {
        // Attempt to get a cached generator first
        GeneratorService generatorService = context.Binder.GeneratorService;
        CachedType? cachedType = context.CachedType;

        IAutoFakerGenerator? cachedGenerator = generatorService.GetGenerator(cachedType);

        if (cachedGenerator != null)
            return cachedGenerator;

        IAutoFakerGenerator generator = CreateGenerator(context);

        // Handle overrides
        List<AutoFakerGeneratorOverride>? configOverrides = context.Config.Overrides;

        if (configOverrides != null)
        {
            List<AutoFakerGeneratorOverride>? overrides = null;

            for (var i = 0; i < configOverrides.Count; i++)
            {
                AutoFakerGeneratorOverride overrideCandidate = configOverrides[i];
                if (overrideCandidate.CanOverride(context))
                {
                    overrides ??= new List<AutoFakerGeneratorOverride>(configOverrides.Count);
                    overrides.Add(overrideCandidate);
                }
            }

            if (overrides != null)
            {
                var newOverrideGenerator = new AutoFakerGeneratorOverrideInvoker(generator, overrides);
                generatorService.SetGenerator(cachedType, newOverrideGenerator);
                return newOverrideGenerator;
            }
        }

        generatorService.SetGenerator(cachedType, generator);
        return generator;
    }

    internal static IAutoFakerGenerator CreateGenerator(AutoFakerContext context)
    {
        CachedType? cachedType = context.CachedType;

        IAutoFakerGenerator? fundamentalGenerator = context.Binder.GeneratorService.GetFundamentalGenerator(cachedType);

        if (fundamentalGenerator != null)
            return fundamentalGenerator;

        // Need check if the type is an in/out parameter and adjusted accordingly
        if (cachedType.IsByRef)
        {
            cachedType = cachedType.GetCachedElementType();
        }

        // Do some type -> generator mapping
        if (cachedType.IsArray)
        {
            cachedType = cachedType.GetCachedElementType();
            return CreateGenericGenerator(CachedTypeService.ArrayGenerator.Value, cachedType);
        }

        if (cachedType.IsEnum)
        {
            return CreateGenericGenerator(CachedTypeService.EnumGenerator.Value, cachedType);
        }

        if (cachedType.IsNullable)
        {
            cachedType = cachedType.GetCachedGenericArguments()![0];
            return CreateGenericGenerator(CachedTypeService.NullableGenerator.Value, cachedType);
        }

        // Check if an expando object needs to generator
        // This actually means an existing dictionary needs to be populated
        if (cachedType.IsExpandoObject)
        {
            return new ExpandoObjectGenerator();
        }

        if (cachedType.IsWeakReference)
        {
            cachedType = cachedType.GetCachedGenericArguments()![0];
            return CreateGenericGenerator(CachedTypeService.WeakReferenceGenerator.Value, cachedType);
        }

        (CachedType? collectionType, GenericCollectionType? genericCollectionType) = GenericTypeUtil.GetGenericCollectionType(cachedType);

        if (collectionType != null)
        {
            // For generic types we need to interrogate the inner types
            CachedType[] generics = collectionType.GetCachedGenericArguments()!;

            switch (genericCollectionType)
            {
                case GenericCollectionType.ImmutableList:
                {
                    CachedType elementType = generics[0];
                    return CreateGenericGenerator(CachedTypeService.ImmutableListGenerator.Value, elementType);
                }
                case GenericCollectionType.ImmutableArray:
                {
                    CachedType elementType = generics[0];
                    return CreateGenericGenerator(CachedTypeService.ImmutableArrayGenerator.Value, elementType);
                }
                case GenericCollectionType.ReadOnlyDictionary:
                {
                    CachedType keyType = generics[0];
                    CachedType valueType = generics[1];

                    return CreateGenericGenerator(CachedTypeService.ReadOnlyDictionaryGenerator.Value, keyType, valueType);
                }
                case GenericCollectionType.ImmutableDictionary:
                {
                    CachedType keyType = generics[0];
                    CachedType valueType = generics[1];

                    return CreateGenericGenerator(CachedTypeService.ImmutableDictionaryGenerator.Value, keyType, valueType);
                }
                case GenericCollectionType.Dictionary:
                case GenericCollectionType.SortedList:
                {
                    CachedType keyType = generics[0];
                    CachedType valueType = generics[1];

                    return CreateGenericGenerator(CachedTypeService.DictionaryGenerator.Value, keyType, valueType);
                }
                case GenericCollectionType.ReadOnlyCollection:
                {
                    CachedType elementType = generics[0];
                    return CreateGenericGenerator(CachedTypeService.ReadOnlyCollectionGenerator.Value, elementType);
                }
                case GenericCollectionType.Collection:
                {
                    CachedType elementType = generics[0];
                    return CreateGenericGenerator(CachedTypeService.CollectionGenerator.Value, elementType);
                }
                case GenericCollectionType.ReadOnlyList:
                case GenericCollectionType.List:
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

        if (BaseDataTableGenerator.TryCreateGenerator(context, cachedType, out BaseDataTableGenerator dataTableGenerator))
        {
            return dataTableGenerator;
        }

        if (BaseDataSetGenerator.TryCreateGenerator(context, cachedType, out BaseDataSetGenerator dataSetGenerator))
        {
            return dataSetGenerator;
        }

        if (cachedType.IsIntellenum)
        {
            return GeneratorService.GetIntellenumGenerator();
        }

        if (cachedType.IsSmartEnum)
        {
            return GeneratorService.GetSmartEnumGenerator();
        }

        return CreateGenericGenerator(CachedTypeService.TypeGenerator.Value, cachedType);
    }

    // We run these additional methods to avoid the params array allocation overhead on common cases
    private static IAutoFakerGenerator CreateGenericGenerator(CachedType genericTypeDefinition, CachedType arg1)
    {
        CachedType closed = genericTypeDefinition.MakeCachedGenericType(arg1)!;
        return (IAutoFakerGenerator)closed.CreateInstance();
    }

    private static IAutoFakerGenerator CreateGenericGenerator(CachedType genericTypeDefinition, CachedType arg1, CachedType arg2)
    {
        CachedType closed = genericTypeDefinition.MakeCachedGenericType(arg1, arg2)!;
        return (IAutoFakerGenerator)closed.CreateInstance();
    }

    private static IAutoFakerGenerator CreateGenericGenerator(CachedType genericTypeDefinition, params CachedType[] genericTypes)
    {
        CachedType closed = genericTypeDefinition.MakeCachedGenericType(genericTypes)!;
        return (IAutoFakerGenerator)closed.CreateInstance();
    }
}