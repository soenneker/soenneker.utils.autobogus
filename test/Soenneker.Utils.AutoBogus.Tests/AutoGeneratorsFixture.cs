using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using AwesomeAssertions;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using Soenneker.Utils.AutoBogus.Generators.Types.DataSets.Base;
using Soenneker.Utils.AutoBogus.Tests.Extensions;
using Soenneker.Utils.AutoBogus.Services;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;
using Soenneker.Utils.AutoBogus.Generators.Types.Enums;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Enums;

namespace Soenneker.Utils.AutoBogus.Tests;

public partial class AutoGeneratorsFixture
{
    public class Factory
    {
        public class ReadOnlyDictionary
        {
            public class ReadOnlyDictionaryBase<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
            {
                public ReadOnlyDictionaryBase(IEnumerable<KeyValuePair<TKey, TValue>> items)
                {
                    foreach (KeyValuePair<TKey, TValue> item in items)
                        _store[item.Key] = item.Value;
                }

                readonly Dictionary<TKey, TValue> _store = [];

                public TValue this[TKey key] => _store[key];
                public IEnumerable<TKey> Keys => _store.Keys;
                public IEnumerable<TValue> Values => _store.Values;
                public int Count => _store.Count;

                public bool ContainsKey(TKey key) => _store.ContainsKey(key);
                public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _store.GetEnumerator();
                public bool TryGetValue(TKey key, out TValue value) => _store.TryGetValue(key, out value);
                IEnumerator IEnumerable.GetEnumerator() => _store.GetEnumerator();
            }

            public class NonGeneric : ReadOnlyDictionaryBase<int, string>
            {
                public NonGeneric(IEnumerable<KeyValuePair<int, string>> items) : base(items)
                {
                }
            }

            public class OneArgument<T> : ReadOnlyDictionaryBase<T, string>
            {
                public OneArgument(IEnumerable<KeyValuePair<T, string>> items) : base(items)
                {
                }
            }

            public class TwoArgumentsThatAreDifferentFromBaseReadOnlyDictionaryClass<TValue, TKey> : ReadOnlyDictionaryBase<TKey, TValue>
            {
                public TwoArgumentsThatAreDifferentFromBaseReadOnlyDictionaryClass(IEnumerable<KeyValuePair<TKey, TValue>> items) : base(items)
                {
                }
            }

            public static IEnumerable<object[]> ListOfReadOnlyDictionaryTypes
            {
                get
                {
                    yield return [typeof(NonGeneric)];
                    yield return [typeof(OneArgument<int>)];
                    yield return [typeof(TwoArgumentsThatAreDifferentFromBaseReadOnlyDictionaryClass<string, int>)];
                }
            }

            [Test]
            [MethodDataSource(nameof(ListOfReadOnlyDictionaryTypes))]
            public void Should_Handle_Subclasses(Type readOnlyDictionaryType)
            {
                // Arrange
                var autoFaker = new AutoFaker();
                autoFaker.Initialize();

                var context = new AutoFakerContext(autoFaker, autoFaker.CacheService.Cache.GetCachedType(readOnlyDictionaryType));

                // Act
                IAutoFakerGenerator generator = AutoFakerGeneratorFactory.CreateGenerator(context);

                object instance = generator.Generate(context);

                // Arrange
                generator.Should().BeOfType<ReadOnlyDictionaryGenerator<int, string>>();

                instance.Should().BeOfType(readOnlyDictionaryType);
            }
        }

        public class Dictionary
        {
            public class NonGeneric : Dictionary<int, string>;

            public class OneArgument<T> : Dictionary<T, string>;

            public class TwoArgumentsThatAreDifferentFromBaseDictionaryClass<TValue, TKey> : Dictionary<TKey, TValue>;

            public static IEnumerable<object[]> ListOfDictionaryTypes
            {
                get
                {
                    yield return [typeof(NonGeneric)];
                    yield return [typeof(OneArgument<int>)];
                    yield return [typeof(TwoArgumentsThatAreDifferentFromBaseDictionaryClass<string, int>)];
                }
            }

            [Test]
            [MethodDataSource(nameof(ListOfDictionaryTypes))]
            public void Should_Handle_Subclasses(Type dictionaryType)
            {
                // Arrange
                var autoFaker = new AutoFaker();
                autoFaker.Initialize();

                var context = new AutoFakerContext(autoFaker, autoFaker.CacheService.Cache.GetCachedType(dictionaryType));

                // Act
                IAutoFakerGenerator generator = AutoFakerGeneratorFactory.CreateGenerator(context);

                object instance = generator.Generate(context);

                // Arrange
                generator.Should().BeOfType<DictionaryGenerator<int, string>>();

                instance.Should().BeOfType(dictionaryType);
            }
        }

        public class Set
        {
            public class NonGeneric : HashSet<int>;

            public class GenericWithDifferentType<TType> : HashSet<int>
            {
                public TType Property { get; set; }
            }

            public static IEnumerable<object[]> ListOfSetTypes
            {
                get
                {
                    yield return [typeof(NonGeneric)];
                    yield return [typeof(GenericWithDifferentType<string>)];
                }
            }

            [Test]
            [MethodDataSource(nameof(ListOfSetTypes))]
            public void Should_Handle_Subclasses(Type setType)
            {
                var autoFaker = new AutoFaker();
                autoFaker.Initialize();

                // Arrange
                var context = new AutoFakerContext(autoFaker);

                context.Setup(autoFaker.CacheService.Cache.GetCachedType(setType));

                // Act
                IAutoFakerGenerator generator = AutoFakerGeneratorFactory.CreateGenerator(context);

                object instance = generator.Generate(context);

                // Arrange
                generator.Should().BeOfType<SetGenerator<int>>();

                instance.Should().BeOfType(setType);
            }
        }

        public class List
        {
            public class NonGeneric : List<int>;

            public class GenericWithDifferentType<TType> : List<int>
            {
                public TType Property { get; set; }
            }

            public static IEnumerable<object[]> ListOfListTypes
            {
                get
                {
                    yield return [typeof(NonGeneric)];
                    yield return [typeof(GenericWithDifferentType<string>)];
                }
            }

            [Test]
            [MethodDataSource(nameof(ListOfListTypes))]
            public void Should_Handle_Subclasses(Type listType)
            {
                // Arrange
                var autoFaker = new AutoFaker();
                autoFaker.Initialize();

                var context = new AutoFakerContext(autoFaker);

                context.Setup(autoFaker.CacheService.Cache.GetCachedType(listType));

                // Act
                IAutoFakerGenerator generator = AutoFakerGeneratorFactory.CreateGenerator(context);

                object instance = generator.Generate(context);

                // Arrange
                generator.Should().BeOfType<ListGenerator<int>>();

                instance.Should().BeOfType(listType);
            }
        }
    }

    public class ReferenceTypes : AutoGeneratorsFixture
    {
        private sealed class TestClass
        {
            public TestClass(in int value)
            {
            }
        }

        [Test]
        public void Should_Not_Throw_For_Reference_Types()
        {
            Type type = typeof(TestClass);
            ConstructorInfo constructor = type.GetConstructors().Single();
            ParameterInfo parameter = constructor.GetParameters().Single();
            AutoFakerContext context = CreateContext(parameter.ParameterType);

            Action action = () => AutoFakerGeneratorFactory.GetGenerator(context);
            action.Should().NotThrow();
        }
    }

    public class RegisteredGenerator : AutoGeneratorsFixture
    {
        [Test]
        [MethodDataSource(nameof(GetRegisteredTypes))]
        public void Generate_Should_Return_Value(Type type)
        {
            var autoFaker = new AutoFaker();
            autoFaker.Initialize();

            CachedType cachedType = autoFaker.CacheService!.Cache.GetCachedType(type);
            var binder = new AutoFakerBinder();

            IAutoFakerGenerator generator = binder.GeneratorService.GetFundamentalGenerator(cachedType)!;

            InvokeGenerator(type, generator).Should().BeOfType(type);
        }

        [Test]
        [MethodDataSource(nameof(GetRegisteredTypes))]
        public void GetGenerator_Should_Return_Generator(Type type)
        {
            var autoFaker = new AutoFaker();
            autoFaker.Initialize();

            CachedType cachedType = autoFaker.CacheService!.Cache.GetCachedType(type);
            AutoFakerContext context = CreateContext(type);
            IAutoFakerGenerator generator = context.Binder.GeneratorService.GetFundamentalGenerator(cachedType)!;

            context.Binder.GeneratorService.Clear();

            AutoFakerGeneratorFactory.GetGenerator(context).Should().Be(generator);
        }

        public static IEnumerable<object[]> GetRegisteredTypes()
        {
            return GeneratorService.GetSupportedFundamentalTypes().Select(c => new object[] {c});
        }

        [Test]
        [MethodDataSource(nameof(GetDataSetAndDataTableTypes))]
        public void GetGenerator_Should_Return_Generator_For_DataSets_And_DataTables(Type dataType, Type generatorType)
        {
            // Arrange
            AutoFakerContext context = CreateContext(dataType);

            // Act
            IAutoFakerGenerator generator = AutoFakerGeneratorFactory.GetGenerator(context);

            // Assert
            generator.Should().BeAssignableTo(generatorType);
        }

        public static IEnumerable<object[]> GetDataSetAndDataTableTypes()
        {
            yield return [typeof(System.Data.DataSet), typeof(BaseDataSetGenerator)];
            yield return [typeof(DataSetGeneratorFacet.TypedDataSet), typeof(BaseDataSetGenerator)];
            yield return [typeof(System.Data.DataTable), typeof(BaseDataTableGenerator)];
            yield return [typeof(DataTableGeneratorFacet.TypedDataTable1), typeof(BaseDataTableGenerator)];
            yield return [typeof(DataTableGeneratorFacet.TypedDataTable2), typeof(BaseDataTableGenerator)];
        }
    }

    public class ExpandoObjectGenerator : AutoGeneratorsFixture
    {
        [Test]
        public void Generate_Should_Return_Value()
        {
            Type type = typeof(ExpandoObject);
            var generator = new Generators.Types.ExpandoObjectGenerator();

            dynamic instance = new ExpandoObject();
            dynamic child = new ExpandoObject();

            instance.Property = string.Empty;
            instance.Child = child;

            child.Property = 0;

            InvokeGenerator(type, generator, instance);

            string property = instance.Property;
            int childProperty = instance.Child.Property;

            property.Should().NotBeEmpty();
            childProperty.Should().NotBe(0);
        }

        [Test]
        public void GetGenerator_Should_Return_NullableGenerator()
        {
            Type type = typeof(ExpandoObject);
            AutoFakerContext context = CreateContext(type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<Generators.Types.ExpandoObjectGenerator>();
        }
    }

    public class ArrayGenerator : AutoGeneratorsFixture
    {
        [Test]
        [Arguments(typeof(TestEnum[]))]
        [Arguments(typeof(TestStruct[]))]
        [Arguments(typeof(TestSealedClass[]))]
        [Arguments(typeof(ITestInterface[]))]
        [Arguments(typeof(TestAbstractClass[]))]
        public void Generate_Should_Return_Array(Type type)
        {
            Type? itemType = type.GetElementType();
            AutoFakerBinderService.SetBinder(new AutoFakerBinder());
            IAutoFakerGenerator generator = CreateGenerator(typeof(ArrayGenerator<>), itemType);
            var array = InvokeGenerator(type, generator) as Array;

            array.Should().NotBeNull();
        }

        [Test]
        [Arguments(typeof(TestEnum[]))]
        [Arguments(typeof(TestStruct[]))]
        [Arguments(typeof(TestSealedClass[]))]
        [Arguments(typeof(ITestInterface[]))]
        [Arguments(typeof(TestAbstractClass[]))]
        public void GetGenerator_Should_Return_ArrayGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type? itemType = type.GetElementType();
            Type generatorType = GetGeneratorType(typeof(ArrayGenerator<>), itemType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class EnumGenerator : AutoGeneratorsFixture
    {
        [Test]
        public void Generate_Should_Return_Enum()
        {
            Type type = typeof(TestEnum);
            var generator = new EnumGenerator<TestEnum>();

            InvokeGenerator(type, generator).Should().BeOfType<TestEnum>();
        }

        [Test]
        public void GetGenerator_Should_Return_EnumGenerator()
        {
            Type type = typeof(TestEnum);
            AutoFakerContext context = CreateContext(type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<EnumGenerator<TestEnum>>();
        }
    }

    public class EnumValuesGenerator : AutoGeneratorsFixture
    {
        [Test]
        public void Generate_Should_Return_EnumValues()
        {
            Type type = typeof(OrderStatusEnumValue);
            var generator = new Soenneker.Utils.AutoBogus.Generators.Types.Enums.EnumValuesGenerator();

            object result = InvokeGenerator(type, generator);

            result.Should().NotBeNull();
            result.Should().BeOfType<OrderStatusEnumValue>();
            ((OrderStatusEnumValue)result).Value.Should().BeOneOf(1, 2, 3);
        }

        [Test]
        public void GetGenerator_Should_Return_EnumValuesGenerator()
        {
            Type type = typeof(OrderStatusEnumValue);
            AutoFakerContext context = CreateContext(type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<Soenneker.Utils.AutoBogus.Generators.Types.Enums.EnumValuesGenerator>();
        }
    }

    public class DictionaryGenerator : AutoGeneratorsFixture
    {
        [Test]
        [Arguments(typeof(IDictionary<int, TestEnum>))]
        [Arguments(typeof(IDictionary<int, TestStruct>))]
        [Arguments(typeof(IDictionary<int, TestSealedClass>))]
        [Arguments(typeof(IDictionary<int, ITestInterface>))]
        [Arguments(typeof(IDictionary<int, TestAbstractClass>))]
        [Arguments(typeof(Dictionary<int, TestSealedClass>))]
        [Arguments(typeof(SortedList<int, TestSealedClass>))]
        public void Generate_Should_Return_Dictionary(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type keyType = genericTypes.ElementAt(0);
            Type valueType = genericTypes.ElementAt(1);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder());
            IAutoFakerGenerator generator = CreateGenerator(typeof(DictionaryGenerator<,>), keyType, valueType);
            var dictionary = InvokeGenerator(type, generator) as IDictionary;

            dictionary.Should().NotBeNull();

            foreach (object? key in dictionary.Keys)
            {
                object? value = dictionary[key];

                key.Should().BeOfType(keyType);
                value.Should().BeOfType(valueType);
            }
        }

        [Test]
        [Arguments(typeof(IDictionary<int, TestEnum>))]
        [Arguments(typeof(IDictionary<int, TestStruct>))]
        [Arguments(typeof(IDictionary<int, TestSealedClass>))]
        [Arguments(typeof(IDictionary<int, ITestInterface>))]
        [Arguments(typeof(IDictionary<int, TestAbstractClass>))]
        [Arguments(typeof(Dictionary<int, TestSealedClass>))]
        [Arguments(typeof(SortedList<int, TestSealedClass>))]
        public void GetGenerator_Should_Return_DictionaryGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type keyType = genericTypes.ElementAt(0);
            Type valueType = genericTypes.ElementAt(1);
            Type generatorType = GetGeneratorType(typeof(DictionaryGenerator<,>), keyType, valueType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class ListGenerator : AutoGeneratorsFixture
    {
        [Test]
        [Arguments(typeof(IList<TestEnum>))]
        [Arguments(typeof(IList<TestStruct>))]
        [Arguments(typeof(IList<TestSealedClass>))]
        [Arguments(typeof(IList<ITestInterface>))]
        [Arguments(typeof(IList<TestAbstractClass>))]
        [Arguments(typeof(List<TestEnum>))]
        [Arguments(typeof(List<TestSealedClass>))]
        [Arguments(typeof(List<TestStruct>))]
        [Arguments(typeof(List<ITestInterface>))]
        [Arguments(typeof(List<TestAbstractClass>))]
        public void Generate_Should_Return_List(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder());
            IAutoFakerGenerator generator = CreateGenerator(typeof(ListGenerator<>), itemType);
            var list = InvokeGenerator(type, generator) as IEnumerable;

            list.Should().NotBeNull();
        }

        [Test]
        [Arguments(typeof(ICollection<TestEnum>))]
        [Arguments(typeof(ICollection<TestStruct>))]
        [Arguments(typeof(ICollection<TestSealedClass>))]
        [Arguments(typeof(ICollection<ITestInterface>))]
        [Arguments(typeof(ICollection<TestAbstractClass>))]
        [Arguments(typeof(Collection<TestEnum>))]
        [Arguments(typeof(Collection<TestSealedClass>))]
        [Arguments(typeof(Collection<TestStruct>))]
        [Arguments(typeof(Collection<ITestInterface>))]
        [Arguments(typeof(Collection<TestAbstractClass>))]
        public void Generate_Should_Return_Collection(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder());
            IAutoFakerGenerator generator = CreateGenerator(typeof(CollectionGenerator<>), itemType);
            var collection = InvokeGenerator(type, generator) as IEnumerable;

            collection.Should().NotBeNull();
        }

        [Test]
        [Arguments(typeof(IList<TestEnum>))]
        [Arguments(typeof(IList<TestStruct>))]
        [Arguments(typeof(IList<TestSealedClass>))]
        [Arguments(typeof(IList<ITestInterface>))]
        [Arguments(typeof(IList<TestAbstractClass>))]
        [Arguments(typeof(List<TestEnum>))]
        [Arguments(typeof(List<TestSealedClass>))]
        [Arguments(typeof(List<TestStruct>))]
        [Arguments(typeof(List<ITestInterface>))]
        [Arguments(typeof(List<TestAbstractClass>))]
        public void GetGenerator_Should_Return_ListGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            Type generatorType = GetGeneratorType(typeof(ListGenerator<>), itemType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }

        [Test]
        [Arguments(typeof(ICollection<TestEnum>))]
        [Arguments(typeof(ICollection<TestStruct>))]
        [Arguments(typeof(ICollection<TestSealedClass>))]
        [Arguments(typeof(ICollection<ITestInterface>))]
        [Arguments(typeof(ICollection<TestAbstractClass>))]
        [Arguments(typeof(Collection<TestEnum>))]
        [Arguments(typeof(Collection<TestSealedClass>))]
        [Arguments(typeof(Collection<TestStruct>))]
        [Arguments(typeof(Collection<ITestInterface>))]
        [Arguments(typeof(Collection<TestAbstractClass>))]
        public void GetGenerator_Should_Return_CollectionGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            Type generatorType = GetGeneratorType(typeof(CollectionGenerator<>), itemType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class SetGenerator : AutoGeneratorsFixture
    {
        [Test]
        [Arguments(typeof(ISet<TestEnum>))]
        [Arguments(typeof(ISet<TestStruct>))]
        [Arguments(typeof(ISet<TestSealedClass>))]
        [Arguments(typeof(ISet<ITestInterface>))]
        [Arguments(typeof(ISet<TestAbstractClass>))]
        [Arguments(typeof(HashSet<TestSealedClass>))]
        [Arguments(typeof(IReadOnlySet<TestSealedClass>))]
        public void Generate_Should_Return_Set(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder());
            IAutoFakerGenerator generator = CreateGenerator(typeof(SetGenerator<>), itemType);
            var set = InvokeGenerator(type, generator) as IEnumerable;

            set.Should().NotBeNull();
        }

        [Test]
        [Arguments(typeof(ISet<TestEnum>))]
        [Arguments(typeof(ISet<TestStruct>))]
        [Arguments(typeof(ISet<TestSealedClass>))]
        [Arguments(typeof(ISet<ITestInterface>))]
        [Arguments(typeof(ISet<TestAbstractClass>))]
        [Arguments(typeof(HashSet<TestSealedClass>))]
        [Arguments(typeof(IReadOnlySet<TestSealedClass>))]
        public void GetGenerator_Should_Return_SetGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder());
            Type generatorType = GetGeneratorType(typeof(SetGenerator<>), itemType);


            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class EnumerableGenerator : AutoGeneratorsFixture
    {
        [Test]
        [Arguments(typeof(IEnumerable<TestEnum>))]
        [Arguments(typeof(IEnumerable<TestStruct>))]
        [Arguments(typeof(IEnumerable<TestSealedClass>))]
        [Arguments(typeof(IEnumerable<ITestInterface>))]
        [Arguments(typeof(IEnumerable<TestAbstractClass>))]
        public void Generate_Should_Return_Enumerable(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder());
            IAutoFakerGenerator generator = CreateGenerator(typeof(EnumerableGenerator<>), itemType);
            var enumerable = InvokeGenerator(type, generator) as IEnumerable;

            enumerable.Should().NotBeNull();
        }

        [Test]
        [Arguments(typeof(IEnumerable<TestEnum>))]
        [Arguments(typeof(IEnumerable<TestStruct>))]
        [Arguments(typeof(IEnumerable<TestSealedClass>))]
        [Arguments(typeof(IEnumerable<ITestInterface>))]
        [Arguments(typeof(IEnumerable<TestAbstractClass>))]
        public void GetGenerator_Should_Return_EnumerableGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            Type generatorType = GetGeneratorType(typeof(EnumerableGenerator<>), itemType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class NullableGenerator : AutoGeneratorsFixture
    {
        [Test]
        public void Generate_Should_Return_Value()
        {
            Type type = typeof(TestEnum?);
            var generator = new NullableGenerator<TestEnum>();

            InvokeGenerator(type, generator).Should().BeOfType<TestEnum>();
        }

        [Test]
        public void GetGenerator_Should_Return_NullableGenerator()
        {
            Type type = typeof(TestEnum?);
            AutoFakerContext context = CreateContext(type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<NullableGenerator<TestEnum>>();
        }
    }

    public class TypeGenerator : AutoGeneratorsFixture
    {
        [Test]
        [Arguments(typeof(TestStruct))]
        [Arguments(typeof(TestSealedClass))]
        [Arguments(typeof(ITestInterface))]
        [Arguments(typeof(TestAbstractClass))]
        public void Generate_Should_Return_Value(Type type)
        {
            AutoFakerBinderService.SetBinder(new AutoFakerBinder());
            IAutoFakerGenerator generator = CreateGenerator(typeof(TypeGenerator<>), type);

            if (type.IsInterface() || type.IsAbstract())
            {
                InvokeGenerator(type, generator).Should().BeNull();
            }
            else
            {
                InvokeGenerator(type, generator).Should().BeAssignableTo(type);
            }
        }

        [Test]
        [Arguments(typeof(TestStruct))]
        [Arguments(typeof(TestSealedClass))]
        [Arguments(typeof(ITestInterface))]
        [Arguments(typeof(TestAbstractClass))]
        public void GetGenerator_Should_Return_TypeGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type generatorType = GetGeneratorType(typeof(TypeGenerator<>), type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class GeneratorOverrides : AutoGeneratorsFixture
    {
        private AutoFakerGeneratorOverride _autoFakerGeneratorOverride;
        private List<AutoFakerGeneratorOverride> _overrides;

        private class TestAutoFakerGeneratorOverride : AutoFakerGeneratorOverride
        {
            public TestAutoFakerGeneratorOverride(bool shouldOverride = false)
            {
                ShouldOverride = shouldOverride;
            }

            public bool ShouldOverride { get; }

            public override bool CanOverride(AutoFakerContext context)
            {
                return ShouldOverride;
            }

            public override void Generate(AutoFakerOverrideContext context)
            {
            }
        }

        public GeneratorOverrides()
        {
            _autoFakerGeneratorOverride = new TestAutoFakerGeneratorOverride(true);
            _overrides =
            [
                new TestAutoFakerGeneratorOverride(),

                _autoFakerGeneratorOverride,

                new TestAutoFakerGeneratorOverride()
            ];
        }

        //[Test]
        //public void Should_Return_All_Matching_Overrides()
        //{
        //    var generatorOverride = new TestAutoFakerGeneratorOverride(true);

        //    _overrides.Insert(1, generatorOverride);

        //    GeneratorService.Clear();

        //    AutoFakerContext context = CreateContext(typeof(string), _overrides);
        //    var invoker = AutoFakerGeneratorFactory.GetGenerator(context) as AutoFakerGeneratorOverrideInvoker;

        //    invoker.Overrides.Should().BeEquivalentTo(new[] {generatorOverride, _autoFakerGeneratorOverride});
        //}

        [Test]
        public void Should_Return_Generator_If_No_Matching_Override()
        {
            _overrides = [new TestAutoFakerGeneratorOverride()];

            AutoFakerContext context = CreateContext(typeof(int), _overrides);
            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<IntGenerator>();
        }

        [Test]
        public void Should_Invoke_Generator()
        {
            AutoFakerContext context = CreateContext(typeof(string), _overrides);
            IAutoFakerGenerator generatorOverride = AutoFakerGeneratorFactory.GetGenerator(context);

            generatorOverride.Generate(context).Should().BeOfType<string>().And.NotBeNull();
        }
    }

    private static object InvokeGenerator(Type type, IAutoFakerGenerator generator, object? instance = null)
    {
        AutoFakerContext context = CreateContext(type);
        context.Instance = instance;

        return generator.Generate(context);
    }

    private static Type GetGeneratorType(Type type, params Type[] types)
    {
        return type.MakeGenericType(types);
    }

    private static IAutoFakerGenerator CreateGenerator(Type type, params Type[] types)
    {
        type = GetGeneratorType(type, types);
        return (IAutoFakerGenerator) Activator.CreateInstance(type);
    }

    private static AutoFakerContext CreateContext(Type type, List<AutoFakerGeneratorOverride>? generatorOverrides = null, int? dataTableRowCountFunctor = null)
    {
        var autoFaker = new AutoFaker();
        autoFaker.Initialize();

        if (generatorOverrides != null)
        {
            autoFaker.Config.Overrides = generatorOverrides;
        }

        if (dataTableRowCountFunctor != null)
        {
            autoFaker.Config.DataTableRowCount = dataTableRowCountFunctor.Value;
        }

        return new AutoFakerContext(autoFaker, autoFaker.CacheService!.Cache.GetCachedType(type));
    }
}
