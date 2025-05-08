using System;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;
using Soenneker.Utils.AutoBogus.Tests.Overrides;
using System.Linq;
using System.Reflection;
using Soenneker.Reflection.Cache.Options;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Override;
using Soenneker.Utils.AutoBogus.Tests.Dtos;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Delegates;
using System;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerTests
{
    [Fact]
    public void Generate_order_should_generate()
    {
        var faker = new AutoFaker();

        var order = faker.Generate<Order>();
        order.Should().NotBeNull();
    }

    [Fact]
    public void Generate_dictionary_should_generate()
    {
        var faker = new AutoFaker();

        var dictionary = faker.Generate<Dictionary<int, string>>();
        dictionary.Should().NotBeNull();
    }

    [Fact]
    public void Generate_struct_should_generate()
    {
        var faker = new AutoFaker();

        var structObj = faker.Generate<TestStruct>();
        structObj.Should().NotBeNull();
    }

    [Fact]
    public void Generate_record_should_generate()
    {
        var faker = new AutoFaker();

        var record = faker.Generate<TestRecord>();
        record.Should().NotBeNull();
        record.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_record_struct_should_generate()
    {
        var faker = new AutoFaker();

        var record = faker.Generate<TestRecordStruct>();

        record.Should().NotBeNull();
        record.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_TestClassWithNumber_should_generate()
    {
        var faker = new AutoFaker();

        var testClass = faker.Generate<TestClassWithNumber<int>>();

        testClass.Should().NotBeNull();
        testClass.Number.Should().NotBe(0);
    }

    [Fact]
    public void Generate_BigInteger_should_generate()
    {
        var faker = new AutoFaker();

        var testClass = faker.Generate<TestClassWithBigInteger>();

        testClass.Should().NotBeNull();
        testClass.BigInteger.Should().NotBeNull();
    }

    [Fact]
    public void Generate_Exception_should_generate()
    {
        var faker = new AutoFaker();

        var testClass = faker.Generate<TestClassWithException>();

        testClass.Should().NotBeNull();
        testClass.Exception.Should().NotBeNull();
    }

    [Fact]
    public void Generate_TestClassEmpty_should_generate()
    {
        var faker = new AutoFaker();

        var record = faker.Generate<TestClassEmpty>();
        record.Should().NotBeNull();
    }

    [Fact]
    public void Generate_TestRecordWithRecursiveConstructor_should_generate()
    {
        var faker = new AutoFaker();

        var record = faker.Generate<TestRecordWithRecursiveConstructor>();
        record.Should().NotBeNull();
        record.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_TestClassWithRecursiveConstructor_should_generate()
    {
        var faker = new AutoFaker();

        var classObj = faker.Generate<TestClassWithRecursiveConstructor>();
        classObj.Should().NotBeNull();
        classObj.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_TestClassWithRecursiveEnumerableConstructor_should_generate()
    {
        var faker = new AutoFaker();

        var classObj = faker.Generate<TestClassWithRecursiveEnumerableConstructor>();
        classObj.Should().NotBeNull();
        classObj.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_TestClassIReadOnlyCollection_should_generate()
    {
        var faker = new AutoFaker();

        var classObj = faker.Generate<TestClassIReadOnlyCollection>();
        classObj.Should().NotBeNull();
        classObj.Ints.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_TestClassCollection_should_generate()
    {
        var faker = new AutoFaker();

        var classObj = faker.Generate<TestClassCollection>();
        classObj.Should().NotBeNull();
        classObj.Strings.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_TestClassReadOnlyCollection_should_generate()
    {
        var faker = new AutoFaker();

        var classObj = faker.Generate<TestClassReadOnlyCollection>();
        classObj.Should().NotBeNull();
        classObj.Strings.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_Product_should_generate()
    {
        var faker = new AutoFaker();

        var product = faker.Generate<Product>();
        product.Should().NotBeNull();
        product.GetRevisions.Should().NotBeNullOrEmpty();
        product.ReadOnlySet.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_many_Orders_should_generate()
    {
        var faker = new AutoFaker();

        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < 1000; i++)
        {
            var order = faker.Generate<Order>();
        }

        stopwatch.Stop();
    }

    [Fact]
    public void Generate_many_int_should_generate()
    {
        var faker = new AutoFaker();

        var intList = new List<int>();

        for (var i = 0; i < 1000; i++)
        {
            var generated = faker.Generate<int>();
            intList.Add(generated);
        }

        intList.Count.Should().Be(1000);
    }

    [Fact]
    public void Generate_with_override_should_give_result()
    {
        for (var i = 0; i < 100; i++)
        {
            var config = new AutoFakerConfig
            {
                Overrides =
                [
                    new BaseCustomOrderOverride(),
                    new LongitudeOverride(),
                    new CustomOrderOverride()
                ]
            };

            var autoFaker = new AutoFaker(config);

            var order = autoFaker.Generate<CustomOrder>();
            order.CustomId.Should().Be("Blah");
        }
    }

    [Fact]
    public void Generate_with_default_RepeatCount_should_generate_correct_count()
    {
        var autoFaker = new AutoFaker();

        var order = autoFaker.Generate<CustomOrder>();
        order.Items.Count().Should().Be(1);
    }

    [Fact]
    public void Generate_with_set_RepeatCount_should_generate_correct_count()
    {
        var autoFaker = new AutoFaker
        {
            Config =
            {
                RepeatCount = 3
            }
        };

        var order = autoFaker.Generate<CustomOrder>();
        order.Items.Count().Should().Be(3);
    }

    [Fact]
    public void Generate_with_smartenum_should_generate()
    {
        var config = new AutoFakerConfig
        {
            Overrides =
            [
                new BaseCustomOrderOverride(),
                new LongitudeOverride(),
                new CustomOrderOverride()
            ]
        };

        var autoFaker = new AutoFaker(config);

        var order = autoFaker.Generate<CustomOrder>();

        order.NullableDaysOfWeek.Should().NotBeEmpty();

        order.Longitude.Should().NotBeNull();
        CustomOrder.Constant.Should().Be("Order2x89ei");
    }

    [Fact]
    public void Generate_Stream_should_be_null()
    {
        var faker = new AutoFaker();

        var stream = faker.Generate<Stream>();
        stream.Should().BeNull();
    }

    [Fact]
    public void Generate_MemoryStream_should_be_null()
    {
        var faker = new AutoFaker();

        var stream = faker.Generate<MemoryStream>();
        stream.Should().NotBeNull();
    }

    [Fact]
    public void Generate_Video_should_generate()
    {
        var faker = new AutoFaker();

        var video = faker.Generate<Video>();
        video.Should().NotBeNull();
        video.MemoryStreamsList.Should().NotBeNullOrEmpty();
        video.StreamsArray.Should().BeEmpty();
    }

    [Fact]
    public void Generate_should_generate_internal_property()
    {
        var faker = new AutoFaker();

        var video = faker.Generate<Video>();
        video.Name.Should().NotBeNull();
    }

    [Fact]
    public void Generate_should_generate_internal_field()
    {
        var faker = new AutoFaker();

        var video = faker.Generate<Video>();
        video.Id.Should().NotBeNull();
    }

    [Fact]
    public void Generate_ImmutableList_should_generate()
    {
        var faker = new AutoFaker();

        var immutableListDto = faker.Generate<ImmutableListDto>();
        immutableListDto.Should().NotBeNull();
        immutableListDto.List.Should().NotBeNullOrEmpty();
        immutableListDto.IList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_ImmutableArray_should_generate()
    {
        var faker = new AutoFaker();

        var immutableListDto = faker.Generate<ImmutableArrayDto>();
        immutableListDto.Should().NotBeNull();
        immutableListDto.Array.Should().NotBeNullOrEmpty();
    }

    
    public sealed class ExampleClass
    {
        public string? Value { get; init; }
        public string AlwaysSet { get; init; }
    }

    [Fact]
    public void Generate_RulesSet_Should_Generate_Wtih_Ruleset_And_Oeverride()
    {
        var faker = new ExampleClassFaker();
        
        var withSetNull = faker.Generate("setnull,default");
        
        withSetNull.AlwaysSet.Should().NotBeEmpty();
        withSetNull.Value.Should().BeNull();
    }
    
    [Fact]
    public void Generate_RulesSet_Should_Generate_With_Custom_Faker()
    {
        var faker = new ExampleClassFaker();
        
        var noRuleSets = faker.Generate();
        var setNull = faker.Generate("setnull");
        var setNullAndDefault = faker.Generate("setnull,default");

        noRuleSets.AlwaysSet.Should().NotBeEmpty();
        noRuleSets.Value.Should().NotBeEmpty();
        
        setNull.AlwaysSet.Should().BeNull(); // <-- this is why I use both the default and set null ruleset
        setNull.Value.Should().BeNull();
        
        setNullAndDefault.AlwaysSet.Should().NotBeEmpty();
        setNullAndDefault.Value.Should().BeNull();
    }
    
    public class StringOverride : AutoFakerOverride<string>
    {
        public override void Generate(AutoFakerOverrideContext context)
        {
            context.Instance = BuildStringWithPrefix(context.GenerateName);
        }

        public static string BuildStringWithPrefix(string prefix) =>
            $"{prefix}-{Guid.NewGuid().ToString()}";
    }
    
    public sealed class ExampleClassFaker : AutoFaker<ExampleClass>
    {
        public ExampleClassFaker()
        {
            Config.Overrides ??= new List<AutoFakerGeneratorOverride>();
            Config.Overrides.Add(new StringOverride());
            
            RuleSet("setnull", set => set.RuleFor(property => property.Value, () => null));
        }
    }
    
    [Fact]
    public void Generate_TestClassWithAbstractClassParameter_should_generate()
    {
        var faker = new AutoFaker();

        var obj = faker.Generate<TestClassWithAbstractClassParameter>();
        obj.Should().NotBeNull();
        obj.TestClass.Should().BeNull();
    }

    [Fact]
    public void Generate_TestClassWithDerivedClassParameter_should_generate()
    {
        var faker = new AutoFaker();

        var obj = faker.Generate<TestClassWithDerivedClassParameter>();
        obj.Should().NotBeNull();
        obj.TestClass.Should().NotBeNull();
    }

    [Fact]
    public void TestClassWithFuncCtor_should_be_null()
    {
        var autoFaker = new AutoFaker();
        var obj = autoFaker.Generate<TestClassWithFuncCtor<int>>();
        obj.Should().BeNull();
    }

    [Fact]
    public void Generate_Action_should_be_null()
    {
        AutoFaker generator = new();
        var result = generator.Generate<Action>();
        result.Should().BeNull();
    }

    [Fact]
    public void Generate_ActionString_should_be_null()
    {
        AutoFaker generator = new();
        var result = generator.Generate<Action<string>>();
        result.Should().BeNull();
    }

    [Fact]
    public void Generate_Func_should_be_null()
    {
        AutoFaker generator = new();
        var result = generator.Generate<Func<string, string>>();
        result.Should().BeNull();
    }

    [Fact]
    public void Generate_TestClassWithAutoPropertyAction_should_not_be_null_but_property_should_be()
    {
        AutoFaker generator = new();
        var result = generator.Generate<TestClassWithAutoPropertyAction>();
        result.Should().NotBeNull();
        result.Action.Should().BeNull();
    }

    [Fact]
    public void Generate_TestClassWithAutoPropertyFunc_should_not_be_null_but_property_should_be()
    {
        AutoFaker generator = new();
        var result = generator.Generate<TestClassWithAutoPropertyFunc>();
        result.Should().NotBeNull();
        result.Func.Should().BeNull();
    }

    [Fact]
    public void Generate_TestClassWithReadOnlyStringField_should_be_null()
    {
        var autoFaker = new AutoFaker();
        var obj = autoFaker.Generate<TestClassWithReadOnlyStringField>();
        obj._string.Should().BeNull();
    }

    [Fact]
    public void Generate_TestClassWithPropertyChangedEvent_should_not_be_null()
    {
        var autoFaker = new AutoFaker();
        var obj = autoFaker.Generate<TestClassWithPropertyChangedEvent>();
        obj.Should().NotBeNull();
    }

    [Fact]
    public void Generate_TestClassWithDelegate_should_not_be_null()
    {
        var autoFaker = new AutoFaker();
        var obj = autoFaker.Generate<TestClassWithDelegate>();
        obj.Should().NotBeNull();
    }

    [Fact]
    public void TestClassWithPrivateProperty_should_be_null()
    {
        var autoFaker = new AutoFaker
        {
            Config =
            {
                ReflectionCacheOptions = new ReflectionCacheOptions
                {
                    FieldFlags = BindingFlags.Public
                }
            }
        };

        var obj = autoFaker.Generate<TestClassWithPrivateField>();
        obj.GetValue().Should().BeNull();
    }

    [Fact]
    public void TestClassWithTupleStringString_should_not_be_null()
    {
        var autoFaker = new AutoFaker();

        var obj = autoFaker.Generate<TestClassWithTupleStringString>();
        obj.Value.Item1.Should().NotBeNull();
        obj.Value.Item2.Should().NotBeNull();
    }

    [Fact]
    public void TestClassWithNullableInt_should_not_be_null()
    {
        var autoFaker = new AutoFaker();

        var obj = autoFaker.Generate<TestClassWithNullableInt>();
        obj.Value.Should().NotBeNull();
    }

    [Fact]
    public void UseSeed_should_generate_same_value()
    {
        var faker1 = new AutoFaker();
        faker1.UseSeed(1);
        var value1 = faker1.Generate<int>();

        var faker2 = new AutoFaker();
        faker2.UseSeed(1);
        var value2 = faker2.Generate<int>();

        value1.Should().Be(value2);
    }

    [Fact]
    public void Generate_with_recursive_depth_0_should_generate()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                RecursiveDepth = 0
            }
        };

        var testClass = faker.Generate<TestClassWithRecursiveConstructor>();
        testClass.Name.Should().NotBeNull();
        testClass.Child.Should().BeNull();
    }

    [Fact]
    public void Generate_with_recursive_depth_1_should_generate()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                RecursiveDepth = 1
            }
        };

        var testClass = faker.Generate<TestClassWithRecursiveConstructor>();
        testClass.Name.Should().NotBeNull();
        testClass.Child.Should().NotBeNull();
        testClass.Child.Child.Should().BeNull();
    }

    [Fact]
    public void Generate_with_recursive_depth_2_should_generate()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                RecursiveDepth = 2
            }
        };

        var testClass = faker.Generate<TestClassWithRecursiveConstructor>();
        testClass.Name.Should().NotBeNull();
        testClass.Child.Should().NotBeNull();
        testClass.Child.Child.Should().NotBeNull();
        testClass.Child.Child.Child.Should().BeNull();
    }

    [Fact]
    public void Generate_with_recursive_depth_3_should_generate()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                RecursiveDepth = 3
            }
        };

        var testClass = faker.Generate<TestClassWithRecursiveConstructor>();
        testClass.Name.Should().NotBeNull();
        testClass.Child.Should().NotBeNull();
        testClass.Child.Child.Should().NotBeNull();
        testClass.Child.Child.Child.Should().NotBeNull();
        testClass.Child.Child.Child.Child.Should().BeNull();
    }

    [Fact]
    public void Generate_with_recursive_depth_4_should_generate()
    {
        var faker = new AutoFaker
        {
            Config =
            {
                RecursiveDepth = 4
            }
        };

        var testClass = faker.Generate<TestClassWithRecursiveConstructor>();
        testClass.Name.Should().NotBeNull();
        testClass.Child.Should().NotBeNull();
        testClass.Child.Child.Should().NotBeNull();
        testClass.Child.Child.Child.Should().NotBeNull();
        testClass.Child.Child.Child.Child.Should().NotBeNull();
        testClass.Child.Child.Child.Child.Child.Should().BeNull();
    }
}