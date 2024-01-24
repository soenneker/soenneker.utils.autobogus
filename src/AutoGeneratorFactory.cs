using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Util;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus;

internal static class AutoGeneratorFactory
{
    internal static readonly IDictionary<Type, IAutoGenerator> Generators = new Dictionary<Type, IAutoGenerator>
    {
        {typeof(bool), new BoolGenerator()},
        {typeof(byte), new ByteGenerator()},
        {typeof(char), new CharGenerator()},
        {typeof(DateTime), new DateTimeGenerator()},
        {typeof(DateTimeOffset), new DateTimeOffsetGenerator()},
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

    internal static IAutoGenerator GetGenerator(AutoGenerateContext context)
    {
        IAutoGenerator generator = ResolveGenerator(context);

        // Check if any overrides are available for this generate request
        List<AutoGeneratorOverride> overrides = context.Overrides.Where(o => o.CanOverride(context)).ToList();

        if (overrides.Any())
        {
            return new AutoGeneratorOverrideInvoker(generator, overrides);
        }

        return generator;
    }

    internal static IAutoGenerator ResolveGenerator(AutoGenerateContext context)
    {
        Type? type = context.GenerateType;

        // Need check if the type is an in/out parameter and adjusted accordingly
        if (type.IsByRef)
        {
            type = type.GetElementType();
        }

        // Check if an expando object needs to generator
        // This actually means an existing dictionary needs to be populated
        if (type.IsExpandoObject())
        {
            return new ExpandoObjectGenerator();
        }

        // Do some type -> generator mapping
        if (type.IsArray)
        {
            type = type.GetElementType();
            return CreateGenericGenerator(typeof(ArrayGenerator<>), type);
        }

        if (DataTableGenerator.TryCreateGenerator(type, out DataTableGenerator dataTableGenerator))
        {
            return dataTableGenerator;
        }

        if (DataSetGenerator.TryCreateGenerator(type, out DataSetGenerator dataSetGenerator))
        {
            return dataSetGenerator;
        }

        if (type.IsEnum())
        {
            return CreateGenericGenerator(typeof(EnumGenerator<>), type);
        }

        if (type.IsNullable())
        {
            type = type.GetGenericArguments().Single();
            return CreateGenericGenerator(typeof(NullableGenerator<>), type);
        }

        (Type? collectionType, GenericCollectionType? genericCollectionType) = ReflectionHelper.GetGenericCollectionType(type);

        if (collectionType != null)
        {
            // For generic types we need to interrogate the inner types
            Type[] generics = ReflectionHelper.GetGenericArguments(collectionType);

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
                        if (type == collectionType)
                        {
                            // Not a full list type, we can't fake it if it's anything other than
                            // the actual IEnumerable<T> interface itelf.
                            Type elementType = generics.Single();
                            return CreateGenericGenerator(typeof(EnumerableGenerator<>), elementType);
                        }

                        break;
                    }
            }
        }

        // Resolve the generator from the type
        if (Generators.TryGetValue(type, out IAutoGenerator? generator))
        {
            return generator;
        }

        return CreateGenericGenerator(typeof(TypeGenerator<>), type);
    }

    private static IAutoGenerator CreateDictionaryGenerator(IEnumerable<Type> generics)
    {
        Type keyType = generics.ElementAt(0);
        Type valueType = generics.ElementAt(1);

        return CreateGenericGenerator(typeof(DictionaryGenerator<,>), keyType, valueType);
    }

    private static IAutoGenerator CreateGenericGenerator(Type generatorType, params Type[] genericTypes)
    {
        Type type = generatorType.MakeGenericType(genericTypes);
        return (IAutoGenerator)Activator.CreateInstance(type);
    }
}