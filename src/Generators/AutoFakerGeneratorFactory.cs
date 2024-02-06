using System;
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
        IAutoFakerGenerator? cachedGenerator = GeneratorService.GetGenerator(context.CachedType);

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
            GeneratorService.SetGenerator(context.CachedType, generator);
            return generator;
        }

        var newOverrideGenerator = new AutoFakerGeneratorOverrideInvoker(generator, overrides);

        GeneratorService.SetGenerator(context.CachedType, newOverrideGenerator);
        return newOverrideGenerator;
    }

    internal static IAutoFakerGenerator CreateGenerator(AutoFakerContext context)
    {
        IAutoFakerGenerator? fundamentalGenerator = GeneratorService.GetFundamentalGenerator(context.CachedType);

        if (fundamentalGenerator != null)
            return fundamentalGenerator;

        Type? type = context.GenerateType;

        // Need check if the type is an in/out parameter and adjusted accordingly
        if (context.CachedType.IsByRef)
        {
            type = type.GetElementType();
        }

        // Do some type -> generator mapping
        if (context.CachedType.IsArray)
        {
            type = type.GetElementType();
            return CreateGenericGenerator(CachedTypeService.ArrayGenerator.Value, type);
        }

        if (context.CachedType.IsEnum)
        {
            return CreateGenericGenerator(CachedTypeService.EnumGenerator.Value, type);
        }

        if (context.CachedType.IsNullable)
        {
            type = context.CachedType.GetGenericArguments()![0];
            return CreateGenericGenerator(CachedTypeService.NullableGenerator.Value, type);
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
            Type[] generics = collectionType.GetGenericArguments()!;

            switch (genericCollectionType!.Name)
            {
                case nameof(GenericCollectionType.ReadOnlyDictionary):
                    {
                        Type keyType = generics[0];
                        Type valueType = generics[1];

                        return CreateGenericGenerator(CachedTypeService.ReadOnlyDictionaryGenerator.Value, keyType, valueType);
                    }
                case nameof(GenericCollectionType.ImmutableDictionary):
                case nameof(GenericCollectionType.Dictionary):
                case nameof(GenericCollectionType.SortedList):
                    {
                        Type keyType = generics[0];
                        Type valueType = generics[1];

                        return CreateGenericGenerator(CachedTypeService.DictionaryGenerator.Value, keyType, valueType);
                    }
                case nameof(GenericCollectionType.ReadOnlyList):
                case nameof(GenericCollectionType.ListType):
                case nameof(GenericCollectionType.ReadOnlyCollection):
                case nameof(GenericCollectionType.Collection):
                    {
                        Type elementType = generics[0];
                        return CreateGenericGenerator(CachedTypeService.ListGenerator.Value, elementType);
                    }
                case nameof(GenericCollectionType.Set):
                    {
                        Type elementType = generics[0];
                        return CreateGenericGenerator(CachedTypeService.SetGenerator.Value, elementType);
                    }
                case nameof(GenericCollectionType.Enumerable):
                    {
                        if (collectionType.Type == type)
                        {
                            // Not a full list type, we can't fake it if it's anything other than
                            // the actual IEnumerable<T> interface itelf.
                            Type elementType = generics[0];
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

        return CreateGenericGenerator(CachedTypeService.TypeGenerator.Value, type);
    }

    private static IAutoFakerGenerator CreateGenericGenerator(CachedType cachedType, params Type[] genericTypes)
    {
        CachedType genericCachedType = cachedType.MakeCachedGenericType(genericTypes)!;

        var generator = (IAutoFakerGenerator)genericCachedType.CreateInstance();
        return generator;
    }
}