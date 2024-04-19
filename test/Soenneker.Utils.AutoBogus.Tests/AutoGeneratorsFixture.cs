using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Generators.Types;
using Soenneker.Utils.AutoBogus.Generators.Types.DataSets.Base;
using Soenneker.Utils.AutoBogus.Tests.Extensions;
using Soenneker.Utils.AutoBogus.Services;
using Soenneker.Utils.AutoBogus.Generators.Types.DataTables.Base;

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

                Dictionary<TKey, TValue> _store = [];

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

            [Theory]
            [MemberData(nameof(ListOfReadOnlyDictionaryTypes))]
            public void Should_Handle_Subclasses(Type readOnlyDictionaryType)
            {
                // Arrange
                var autoFaker = new AutoFaker();

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
            public class NonGeneric : Dictionary<int, string>
            {
            }

            public class OneArgument<T> : Dictionary<T, string>
            {
            }

            public class TwoArgumentsThatAreDifferentFromBaseDictionaryClass<TValue, TKey> : Dictionary<TKey, TValue>
            {
            }

            public static IEnumerable<object[]> ListOfDictionaryTypes
            {
                get
                {
                    yield return [typeof(NonGeneric)];
                    yield return [typeof(OneArgument<int>)];
                    yield return [typeof(TwoArgumentsThatAreDifferentFromBaseDictionaryClass<string, int>)];
                }
            }

            [Theory]
            [MemberData(nameof(ListOfDictionaryTypes))]
            public void Should_Handle_Subclasses(Type dictionaryType)
            {
                // Arrange
                var autoFaker = new AutoFaker();

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
            public class NonGeneric : HashSet<int>
            {
            }

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

            [Theory]
            [MemberData(nameof(ListOfSetTypes))]
            public void Should_Handle_Subclasses(Type setType)
            {
                var autoFaker = new AutoFaker();

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
            public class NonGeneric : List<int>
            {
            }

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

            [Theory]
            [MemberData(nameof(ListOfListTypes))]
            public void Should_Handle_Subclasses(Type listType)
            {
                // Arrange
                var autoFaker = new AutoFaker();

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

    public class ReferenceTypes
        : AutoGeneratorsFixture
    {
        private sealed class TestClass
        {
            public TestClass(in int value)
            {
            }
        }

        [Fact]
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

    public class RegisteredGenerator
        : AutoGeneratorsFixture
    {
        [Theory]
        [MemberData(nameof(GetRegisteredTypes))]
        public void Generate_Should_Return_Value(Type type)
        {
            var autoFaker = new AutoFaker();

            CachedType cachedType = autoFaker.CacheService.Cache.GetCachedType(type);
            var binder = new AutoFakerBinder(new AutoFakerConfig());

            IAutoFakerGenerator generator = binder.GeneratorService.GetFundamentalGenerator(cachedType)!;

            InvokeGenerator(type, generator).Should().BeOfType(type);
        }

        [Theory]
        [MemberData(nameof(GetRegisteredTypes))]
        public void GetGenerator_Should_Return_Generator(Type type)
        {
            var autoFaker = new AutoFaker();
            CachedType cachedType = autoFaker.CacheService.Cache.GetCachedType(type);
            AutoFakerContext context = CreateContext(type);
            IAutoFakerGenerator generator = context.Binder.GeneratorService.GetFundamentalGenerator(cachedType)!;

            context.Binder.GeneratorService.Clear();

            AutoFakerGeneratorFactory.GetGenerator(context).Should().Be(generator);
        }

        public static IEnumerable<object[]> GetRegisteredTypes()
        {
            return GeneratorService.GetSupportedFundamentalTypes().Select(c => new object[] {c});
        }

        [Theory]
        [MemberData(nameof(GetDataSetAndDataTableTypes))]
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

    public class ExpandoObjectGenerator
        : AutoGeneratorsFixture
    {
        [Fact]
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

        [Fact]
        public void GetGenerator_Should_Return_NullableGenerator()
        {
            Type type = typeof(ExpandoObject);
            AutoFakerContext context = CreateContext(type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<Generators.Types.ExpandoObjectGenerator>();
        }
    }

    public class ArrayGenerator
        : AutoGeneratorsFixture
    {
        [Theory]
        [InlineData(typeof(TestEnum[]))]
        [InlineData(typeof(TestStruct[]))]
        [InlineData(typeof(TestClass[]))]
        [InlineData(typeof(ITestInterface[]))]
        [InlineData(typeof(TestAbstractClass[]))]
        public void Generate_Should_Return_Array(Type type)
        {
            Type? itemType = type.GetElementType();
            AutoFakerBinderService.SetBinder(new AutoFakerBinder(new AutoFakerConfig()));
            IAutoFakerGenerator generator = CreateGenerator(typeof(ArrayGenerator<>), itemType);
            var array = InvokeGenerator(type, generator) as Array;

            array.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(TestEnum[]))]
        [InlineData(typeof(TestStruct[]))]
        [InlineData(typeof(TestClass[]))]
        [InlineData(typeof(ITestInterface[]))]
        [InlineData(typeof(TestAbstractClass[]))]
        public void GetGenerator_Should_Return_ArrayGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type? itemType = type.GetElementType();
            Type generatorType = GetGeneratorType(typeof(ArrayGenerator<>), itemType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class EnumGenerator
        : AutoGeneratorsFixture
    {
        [Fact]
        public void Generate_Should_Return_Enum()
        {
            Type type = typeof(TestEnum);
            var generator = new EnumGenerator<TestEnum>();

            InvokeGenerator(type, generator).Should().BeOfType<TestEnum>();
        }

        [Fact]
        public void GetGenerator_Should_Return_EnumGenerator()
        {
            Type type = typeof(TestEnum);
            AutoFakerContext context = CreateContext(type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<EnumGenerator<TestEnum>>();
        }
    }

    public class DictionaryGenerator
        : AutoGeneratorsFixture
    {
        [Theory]
        [InlineData(typeof(IDictionary<int, TestEnum>))]
        [InlineData(typeof(IDictionary<int, TestStruct>))]
        [InlineData(typeof(IDictionary<int, TestClass>))]
        [InlineData(typeof(IDictionary<int, ITestInterface>))]
        [InlineData(typeof(IDictionary<int, TestAbstractClass>))]
        [InlineData(typeof(Dictionary<int, TestClass>))]
        [InlineData(typeof(SortedList<int, TestClass>))]
        public void Generate_Should_Return_Dictionary(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type keyType = genericTypes.ElementAt(0);
            Type valueType = genericTypes.ElementAt(1);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder(new AutoFakerConfig()));
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

        [Theory]
        [InlineData(typeof(IDictionary<int, TestEnum>))]
        [InlineData(typeof(IDictionary<int, TestStruct>))]
        [InlineData(typeof(IDictionary<int, TestClass>))]
        [InlineData(typeof(IDictionary<int, ITestInterface>))]
        [InlineData(typeof(IDictionary<int, TestAbstractClass>))]
        [InlineData(typeof(Dictionary<int, TestClass>))]
        [InlineData(typeof(SortedList<int, TestClass>))]
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

    public class ListGenerator: AutoGeneratorsFixture
    {
        [Theory]
        [InlineData(typeof(IList<TestEnum>))]
        [InlineData(typeof(IList<TestStruct>))]
        [InlineData(typeof(IList<TestClass>))]
        [InlineData(typeof(IList<ITestInterface>))]
        [InlineData(typeof(IList<TestAbstractClass>))]
        [InlineData(typeof(List<TestEnum>))]
        [InlineData(typeof(List<TestClass>))]
        [InlineData(typeof(List<TestStruct>))]
        [InlineData(typeof(List<ITestInterface>))]
        [InlineData(typeof(List<TestAbstractClass>))]
        public void Generate_Should_Return_List(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder(new AutoFakerConfig()));
            IAutoFakerGenerator generator = CreateGenerator(typeof(ListGenerator<>), itemType);
            var list = InvokeGenerator(type, generator) as IEnumerable;

            list.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(ICollection<TestEnum>))]
        [InlineData(typeof(ICollection<TestStruct>))]
        [InlineData(typeof(ICollection<TestClass>))]
        [InlineData(typeof(ICollection<ITestInterface>))]
        [InlineData(typeof(ICollection<TestAbstractClass>))]
        [InlineData(typeof(Collection<TestEnum>))]
        [InlineData(typeof(Collection<TestClass>))]
        [InlineData(typeof(Collection<TestStruct>))]
        [InlineData(typeof(Collection<ITestInterface>))]
        [InlineData(typeof(Collection<TestAbstractClass>))]
        public void Generate_Should_Return_Collection(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder(new AutoFakerConfig()));
            IAutoFakerGenerator generator = CreateGenerator(typeof(CollectionGenerator<>), itemType);
            var collection = InvokeGenerator(type, generator) as IEnumerable;

            collection.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(IList<TestEnum>))]
        [InlineData(typeof(IList<TestStruct>))]
        [InlineData(typeof(IList<TestClass>))]
        [InlineData(typeof(IList<ITestInterface>))]
        [InlineData(typeof(IList<TestAbstractClass>))]
        [InlineData(typeof(List<TestEnum>))]
        [InlineData(typeof(List<TestClass>))]
        [InlineData(typeof(List<TestStruct>))]
        [InlineData(typeof(List<ITestInterface>))]
        [InlineData(typeof(List<TestAbstractClass>))]
        public void GetGenerator_Should_Return_ListGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            Type generatorType = GetGeneratorType(typeof(ListGenerator<>), itemType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }

        [Theory]
        [InlineData(typeof(ICollection<TestEnum>))]
        [InlineData(typeof(ICollection<TestStruct>))]
        [InlineData(typeof(ICollection<TestClass>))]
        [InlineData(typeof(ICollection<ITestInterface>))]
        [InlineData(typeof(ICollection<TestAbstractClass>))]
        [InlineData(typeof(Collection<TestEnum>))]
        [InlineData(typeof(Collection<TestClass>))]
        [InlineData(typeof(Collection<TestStruct>))]
        [InlineData(typeof(Collection<ITestInterface>))]
        [InlineData(typeof(Collection<TestAbstractClass>))]
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
        [Theory]
        [InlineData(typeof(ISet<TestEnum>))]
        [InlineData(typeof(ISet<TestStruct>))]
        [InlineData(typeof(ISet<TestClass>))]
        [InlineData(typeof(ISet<ITestInterface>))]
        [InlineData(typeof(ISet<TestAbstractClass>))]
        [InlineData(typeof(HashSet<TestClass>))]
        [InlineData(typeof(IReadOnlySet<TestClass>))]
        public void Generate_Should_Return_Set(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder(new AutoFakerConfig()));
            IAutoFakerGenerator generator = CreateGenerator(typeof(SetGenerator<>), itemType);
            var set = InvokeGenerator(type, generator) as IEnumerable;

            set.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(ISet<TestEnum>))]
        [InlineData(typeof(ISet<TestStruct>))]
        [InlineData(typeof(ISet<TestClass>))]
        [InlineData(typeof(ISet<ITestInterface>))]
        [InlineData(typeof(ISet<TestAbstractClass>))]
        [InlineData(typeof(HashSet<TestClass>))]
        [InlineData(typeof(IReadOnlySet<TestClass>))]
        public void GetGenerator_Should_Return_SetGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder(new AutoFakerConfig()));
            Type generatorType = GetGeneratorType(typeof(SetGenerator<>), itemType);


            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class EnumerableGenerator
        : AutoGeneratorsFixture
    {
        [Theory]
        [InlineData(typeof(IEnumerable<TestEnum>))]
        [InlineData(typeof(IEnumerable<TestStruct>))]
        [InlineData(typeof(IEnumerable<TestClass>))]
        [InlineData(typeof(IEnumerable<ITestInterface>))]
        [InlineData(typeof(IEnumerable<TestAbstractClass>))]
        public void Generate_Should_Return_Enumerable(Type type)
        {
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            AutoFakerBinderService.SetBinder(new AutoFakerBinder(new AutoFakerConfig()));
            IAutoFakerGenerator generator = CreateGenerator(typeof(EnumerableGenerator<>), itemType);
            var enumerable = InvokeGenerator(type, generator) as IEnumerable;

            enumerable.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(IEnumerable<TestEnum>))]
        [InlineData(typeof(IEnumerable<TestStruct>))]
        [InlineData(typeof(IEnumerable<TestClass>))]
        [InlineData(typeof(IEnumerable<ITestInterface>))]
        [InlineData(typeof(IEnumerable<TestAbstractClass>))]
        public void GetGenerator_Should_Return_EnumerableGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type[] genericTypes = type.GetGenericArguments();
            Type itemType = genericTypes.ElementAt(0);
            Type generatorType = GetGeneratorType(typeof(EnumerableGenerator<>), itemType);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class NullableGenerator
        : AutoGeneratorsFixture
    {
        [Fact]
        public void Generate_Should_Return_Value()
        {
            Type type = typeof(TestEnum?);
            var generator = new NullableGenerator<TestEnum>();

            InvokeGenerator(type, generator).Should().BeOfType<TestEnum>();
        }

        [Fact]
        public void GetGenerator_Should_Return_NullableGenerator()
        {
            Type type = typeof(TestEnum?);
            AutoFakerContext context = CreateContext(type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<NullableGenerator<TestEnum>>();
        }
    }

    public class TypeGenerator
        : AutoGeneratorsFixture
    {
        [Theory]
        [InlineData(typeof(TestStruct))]
        [InlineData(typeof(TestClass))]
        [InlineData(typeof(ITestInterface))]
        [InlineData(typeof(TestAbstractClass))]
        public void Generate_Should_Return_Value(Type type)
        {
            AutoFakerBinderService.SetBinder(new AutoFakerBinder(new AutoFakerConfig()));
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

        [Theory]
        [InlineData(typeof(TestStruct))]
        [InlineData(typeof(TestClass))]
        [InlineData(typeof(ITestInterface))]
        [InlineData(typeof(TestAbstractClass))]
        public void GetGenerator_Should_Return_TypeGenerator(Type type)
        {
            AutoFakerContext context = CreateContext(type);
            Type generatorType = GetGeneratorType(typeof(TypeGenerator<>), type);

            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
        }
    }

    public class GeneratorOverrides
        : AutoGeneratorsFixture
    {
        private AutoFakerGeneratorOverride _autoFakerGeneratorOverride;
        private List<AutoFakerGeneratorOverride> _overrides;

        private class TestAutoFakerGeneratorOverride
            : AutoFakerGeneratorOverride
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

        //[Fact]
        //public void Should_Return_All_Matching_Overrides()
        //{
        //    var generatorOverride = new TestAutoFakerGeneratorOverride(true);

        //    _overrides.Insert(1, generatorOverride);

        //    GeneratorService.Clear();

        //    AutoFakerContext context = CreateContext(typeof(string), _overrides);
        //    var invoker = AutoFakerGeneratorFactory.GetGenerator(context) as AutoFakerGeneratorOverrideInvoker;

        //    invoker.Overrides.Should().BeEquivalentTo(new[] {generatorOverride, _autoFakerGeneratorOverride});
        //}

        [Fact]
        public void Should_Return_Generator_If_No_Matching_Override()
        {
            _overrides = [new TestAutoFakerGeneratorOverride()];

            AutoFakerContext context = CreateContext(typeof(int), _overrides);
            AutoFakerGeneratorFactory.GetGenerator(context).Should().BeOfType<IntGenerator>();
        }

        [Fact]
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

        if (generatorOverrides != null)
        {
            autoFaker.Config.Overrides = generatorOverrides;
        }

        if (dataTableRowCountFunctor != null)
        {
            autoFaker.Config.DataTableRowCount = dataTableRowCountFunctor.Value;
        }

        return new AutoFakerContext(autoFaker, autoFaker.CacheService.Cache.GetCachedType(type));
    }
}