using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Util;

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
        {typeof(IPAddress), new IPAddressGenerator()},
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
        if (ReflectionHelper.IsExpandoObject(type))
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

        if (ReflectionHelper.IsEnum(type))
        {
            return CreateGenericGenerator(typeof(EnumGenerator<>), type);
        }

        if (ReflectionHelper.IsNullable(type))
        {
            type = ReflectionHelper.GetGenericArguments(type).Single();
            return CreateGenericGenerator(typeof(NullableGenerator<>), type);
        }

        Type genericCollectionType = ReflectionHelper.GetGenericCollectionType(type);

        if (genericCollectionType != null)
        {
            // For generic types we need to interrogate the inner types
            IEnumerable<Type> generics = ReflectionHelper.GetGenericArguments(genericCollectionType);

            if (ReflectionHelper.IsReadOnlyDictionary(genericCollectionType))
            {
                Type keyType = generics.ElementAt(0);
                Type valueType = generics.ElementAt(1);

                return CreateGenericGenerator(typeof(ReadOnlyDictionaryGenerator<,>), keyType, valueType);
            }

            if (ReflectionHelper.IsDictionary(genericCollectionType))
            {
                return CreateDictionaryGenerator(generics);
            }

            if (ReflectionHelper.IsList(genericCollectionType))
            {
                Type elementType = generics.Single();
                return CreateGenericGenerator(typeof(ListGenerator<>), elementType);
            }

            if (ReflectionHelper.IsSet(genericCollectionType))
            {
                Type elementType = generics.Single();
                return CreateGenericGenerator(typeof(SetGenerator<>), elementType);
            }

            if (ReflectionHelper.IsCollection(genericCollectionType))
            {
                Type elementType = generics.Single();
                return CreateGenericGenerator(typeof(ListGenerator<>), elementType);
            }

            if (ReflectionHelper.IsEnumerable(genericCollectionType))
            {
                // Not a full list type, we can't fake it if it's anything other than
                // the actual IEnumerable<T> interface itelf.
                if (type == genericCollectionType)
                {
                    Type elementType = generics.Single();
                    return CreateGenericGenerator(typeof(EnumerableGenerator<>), elementType);
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