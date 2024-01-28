using System;
using System.Collections.Generic;
using System.Net;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using Soenneker.Utils.AutoBogus.Utils;

namespace Soenneker.Utils.AutoBogus.Generators;

internal static class GeneratorFactory
{
    internal static readonly IDictionary<Type, IAutoFakerGenerator> Generators = new Dictionary<Type, IAutoFakerGenerator>
    {
        {typeof(bool), new BoolGenerator()},
        {typeof(byte), new ByteGenerator()},
        {typeof(char), new CharGenerator()},
        {typeof(DateTime), new DateTimeGenerator()},
        {typeof(DateTimeOffset), new DateTimeOffsetGenerator()},
        {typeof(DateOnly), new DateOnlyGenerator()},
        {typeof(TimeOnly), new TimeOnlyGenerator()},
        {typeof(decimal), new DecimalGenerator()},
        {typeof(double), new DoubleGenerator()},
        {typeof(float), new FloatGenerator()},
        {typeof(Guid), new GuidGenerator()},
        {typeof(int), new IntGenerator()},
        {typeof(IPAddress), new IpAddressGenerator()},
        {typeof(long), new LongGenerator()},
        {typeof(sbyte), new SByteGenerator()},
        {typeof(short), new ShortGenerator()},
        {typeof(string), new StringGenerator()},
        {typeof(uint), new UIntGenerator()},
        {typeof(ulong), new ULongGenerator()},
        {typeof(Uri), new UriGenerator()},
        {typeof(ushort), new UShortGenerator()}
    };

    internal static IAutoFakerGenerator GetGenerator(AutoFakerContext context)
    {
        IAutoFakerGenerator generator = ResolveGenerator(context);

        List<GeneratorOverride>? overrides = null;

        if (context.Overrides != null)
        {
            overrides = [];

            for (var i = 0; i < context.Overrides.Count; i++)
            {
                GeneratorOverride? o = context.Overrides[i];

                if (o.CanOverride(context))
                    overrides.Add(o);
            }
        }

        if (overrides == null || overrides.Count == 0)
            return generator;

        return new GeneratorOverrideInvoker(generator, overrides);
    }

    internal static IAutoFakerGenerator ResolveGenerator(AutoFakerContext context)
    {
        Type? type = context.GenerateType;

        // Need check if the type is an in/out parameter and adjusted accordingly
        if (type.IsByRef)
        {
            type = type.GetElementType();
        }

        // Do some type -> generator mapping
        if (type.IsArray)
        {
            type = type.GetElementType();
            return CreateGenericGenerator(typeof(ArrayGenerator<>), type);
        }

        // Resolve the generator from the type
        if (Generators.TryGetValue(type, out IAutoFakerGenerator? generator))
        {
            return generator;
        }

        if (context.CachedType.IsEnum)
        {
            return CreateGenericGenerator(typeof(EnumGenerator<>), type);
        }

        if (context.CachedType.IsNullable)
        {
            type = context.CachedType.GetGenericArguments()![0];
            return CreateGenericGenerator(typeof(NullableGenerator<>), type);
        }

        // Check if an expando object needs to generator
        // This actually means an existing dictionary needs to be populated
        if (context.CachedType.IsExpandoObject())
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

                    return CreateGenericGenerator(typeof(ReadOnlyDictionaryGenerator<,>), keyType, valueType);
                }
                case nameof(GenericCollectionType.ImmutableDictionary):
                case nameof(GenericCollectionType.Dictionary):
                case nameof(GenericCollectionType.SortedList):
                {
                    return CreateDictionaryGenerator(generics);
                }
                case nameof(GenericCollectionType.ReadOnlyList):
                case nameof(GenericCollectionType.ListType):
                case nameof(GenericCollectionType.ReadOnlyCollection):
                case nameof(GenericCollectionType.Collection):
                {
                    Type elementType = generics[0];
                    return CreateGenericGenerator(typeof(ListGenerator<>), elementType);
                }
                case nameof(GenericCollectionType.Set):
                {
                    Type elementType = generics[0];
                    return CreateGenericGenerator(typeof(SetGenerator<>), elementType);
                }
                case nameof(GenericCollectionType.Enumerable):
                {
                    if (collectionType.Type == type)
                    {
                        // Not a full list type, we can't fake it if it's anything other than
                        // the actual IEnumerable<T> interface itelf.
                        Type elementType = generics[0];
                        return CreateGenericGenerator(typeof(EnumerableGenerator<>), elementType);
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

        return CreateGenericGenerator(typeof(TypeGenerator<>), type);
    }

    private static IAutoFakerGenerator CreateDictionaryGenerator(Type[] generics)
    {
        Type keyType = generics[0];
        Type valueType = generics[1];

        return CreateGenericGenerator(typeof(DictionaryGenerator<,>), keyType, valueType);
    }

    private static IAutoFakerGenerator CreateGenericGenerator(Type generatorType, params Type[] genericTypes)
    {
        Type type = generatorType.MakeGenericType(genericTypes);
        return (IAutoFakerGenerator) Activator.CreateInstance(type);
    }
}