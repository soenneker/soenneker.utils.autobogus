using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Services;
using Soenneker.Utils.AutoBogus.Tests.Dtos;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerFixture
{
    private static readonly Type _type = typeof(AutoFaker);

    public class Configure
        : AutoFakerFixture
    {
        [Fact]
        public void Should_Configure_Default_Config()
        {
            AutoFakerConfig? fakerConfig = null;

            var autoFaker = new AutoFaker();

            autoFaker.Configure(builder =>
            {
                var instance = builder as AutoFakerConfigBuilder;
                fakerConfig = instance._autoFakerConfig; 
            });

            fakerConfig.Should().Be(autoFaker.Config);
        }
    }

    public class Create
        : AutoFakerFixture
    {
        [Fact]
        public void Should_Configure_Child_Config()
        {
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(new AutoFakerConfig());

            var autoFaker = new AutoFaker(configure);
            autoFaker.Should().BeOfType<AutoFaker>();
        }
    }

    public class Generate_Instance: AutoFakerFixture
    {
        private static MethodInfo _generate = _type.GetMethod("Generate", BindingFlags.Instance | BindingFlags.Public, null, [typeof(Action<IAutoGenerateConfigBuilder>)], null)!;
        private static MethodInfo _generateMany = _type.GetMethod("Generate", BindingFlags.Instance | BindingFlags.Public, null, [typeof(int), typeof(Action<IAutoGenerateConfigBuilder>)], null)!;

        private readonly AutoFaker _autoFaker;
        private readonly AutoFakerConfig _fakerConfig;

        public Generate_Instance()
        {
            var autoFaker = new AutoFaker();
            _fakerConfig = autoFaker.Config;
            _autoFaker = autoFaker;
        }

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Should_Generate_Type(Type type)
        {
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(_fakerConfig);
            AssertGenerate(type, _generate, _autoFaker, configure);
        }

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Should_Generate_Many_Types(Type type)
        {   
            int count = AutoFakerDefaultConfigOptions.DefaultRepeatCount;
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(_fakerConfig);

            AssertGenerateMany(type, _generateMany, _autoFaker, count, configure);
        }

        [Fact]
        public void Should_Generate_Complex_Type()
        {
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(_fakerConfig);
            var generatedOrder = _autoFaker.Generate<Order>(configure);

            generatedOrder.Should().BeGeneratedWithoutMocks();
        }

        [Fact]
        public void Should_Generate_Many_Complex_Types()
        {
            int count = AutoFakerDefaultConfigOptions.DefaultRepeatCount;
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(_fakerConfig);
            List<Order> instances = _autoFaker.Generate<Order>(count, configure);

            AssertGenerateMany(instances);
        }
    }

    public class Generate_Static
        : AutoFakerFixture
    {
        private static readonly MethodInfo _generate = _type.GetMethod("GenerateStatic", BindingFlags.Static | BindingFlags.Public, null, [typeof(Action<IAutoGenerateConfigBuilder>)], null);
        private static readonly MethodInfo _generateMany = _type.GetMethod("GenerateStatic", BindingFlags.Static | BindingFlags.Public, null, [typeof(int), typeof(Action<IAutoGenerateConfigBuilder>)], null);

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Should_Generate_Type(Type type)
        {
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(new AutoFakerConfig());
            AssertGenerate(type, _generate, null, configure);
        }

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Should_Generate_Many_Types(Type type)
        {
            int count = AutoFakerDefaultConfigOptions.DefaultRepeatCount;
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(new AutoFakerConfig());

            AssertGenerateMany(type, _generateMany, null, count, configure);
        }

        [Fact]
        public void Should_Generate_Complex_Type()
        {
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(new AutoFakerConfig());
            AutoFaker.GenerateStatic<Order>(configure).Should().BeGeneratedWithoutMocks();
        }

        [Fact]
        public void Should_Generate_Many_Complex_Types()
        {
            int count = AutoFakerDefaultConfigOptions.DefaultRepeatCount;
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(new AutoFakerConfig());
            List<Order> instances = AutoFaker.GenerateStatic<Order>(count, configure);

            AssertGenerateMany(instances);
        }
    }

    //public class AutoFaker_WithFakerHub : AutoFakerFixture
    //{
    //    [Fact]
    //    public void Should_Use_Caller_Supplied_Faker()
    //    {
    //        // We infer that our FakerHub was used by reseeding it and testing that we get the same sequence both times.
    //        var faker = new Faker();

    //        Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(
    //            new AutoFakerConfig(),
    //            builder => builder.WithFaker(faker));

    //        IAutoFaker autoFaker = AutoFaker.Create(configure);

    //        faker.Random = new Randomizer(1);

    //        var instance1 = autoFaker.Generate<TestObject>();

    //        faker.Random = new Randomizer(1);

    //        var instance2 = autoFaker.Generate<TestObject>();

    //        instance1.Should().BeEquivalentTo(instance2);
    //    }

    //    class TestObject
    //    {
    //        public int IntegerValue;
    //        public string StringValue;
    //    }
    //}

    public class Behaviors_Skip : AutoFakerFixture
    {
        //[Fact]
        //public void Should_Skip_Configured_Types()
        //{
        //    var instance = AutoFaker.Generate<Order>(builder =>
        //    {
        //        builder
        //            .WithSkip<ICalculator>()
        //            .WithSkip<Guid?>();
        //    });

        //    instance.Calculator.Should().BeNull();
        //    instance.Code.Should().BeNull();
        //}

        //[Fact]
        //public void Should_Skip_Configured_Members()
        //{
        //    var instance = AutoFaker.Generate<Order>(builder =>
        //    {
        //        builder
        //            .WithSkip<Order>(o => o.Discounts)
        //            .WithSkip<OrderItem>(i => i.Discounts);
        //    });

        //    instance.Discounts.Should().BeNull();
        //    instance.Items.Should().OnlyContain(i => i.Discounts == null);
        //}
    }

    public class Behaviors_Types
        : AutoFakerFixture
    {
        [Fact]
        public void Should_Not_Generate_Interface_Type()
        {
            AutoFaker.GenerateStatic<ITestInterface>().Should().BeNull();
        }

        [Fact]
        public void Should_Not_Generate_Abstract_Class_Type()
        {
            AutoFaker.GenerateStatic<TestAbstractClass>().Should().BeNull();
        }
    }

    public class Behaviors_Recursive
        : AutoFakerFixture
    {
        private readonly TestRecursiveClass _instance;

        public Behaviors_Recursive()
        {
            _instance = AutoFaker.GenerateStatic<TestRecursiveClass>(builder =>
            {
                builder.WithRecursiveDepth(3);
            });
        }

        [Fact]
        public void Should_Generate_Recursive_Types()
        {
            _instance.Child.Should().NotBeNull();
            _instance.Child.Child.Should().NotBeNull();
            _instance.Child.Child.Child.Should().NotBeNull();
            _instance.Child.Child.Child.Child.Should().BeNull();
        }

        [Fact]
        public void Should_Generate_Recursive_Lists()
        {
            IEnumerable<TestRecursiveClass> children = _instance.Children;
            List<TestRecursiveClass> children1 = children.SelectMany(c => c.Children).ToList();
            List<TestRecursiveClass> children2 = children1.SelectMany(c => c.Children).ToList();
            List<TestRecursiveClass> children3 = children2.Where(c => c.Children != null).ToList();

            children.Should().HaveCount(3);
            children1.Should().HaveCount(9);
            children2.Should().HaveCount(27);
            children3.Should().HaveCount(0);
        }

        [Fact]
        public void Should_Generate_Recursive_Sub_Types()
        {
            _instance.Sub.Should().NotBeNull();
            _instance.Sub.Value.Sub.Should().NotBeNull();
            _instance.Sub.Value.Sub.Value.Sub.Should().NotBeNull();
            _instance.Sub.Value.Sub.Value.Sub.Value.Sub.Should().BeNull();
        }
    }

    public static IEnumerable<object[]> GetTypes()
    {
        foreach (Type? type in GeneratorService.GetSupportedFundamentalTypes())
        {
            yield return [type];
        }

        yield return [typeof(string[])];
        yield return [typeof(TestEnum)];
        yield return [typeof(IDictionary<Guid, TestStruct>)];
        yield return [typeof(IEnumerable<TestClass>)];
        yield return [typeof(int?)];
    }

    private static Action<TBuilder> CreateConfigure<TBuilder>(AutoFakerConfig assertFakerConfig, Action<TBuilder>? configure = null)
    {
        return builder =>
        {
            configure?.Invoke(builder);

            //var instance = builder as AutoFakerConfigBuilder;
            //instance._autoFakerConfig.Should().NotBe(assertFakerConfig);
        };
    }

    public static void AssertGenerate(Type type, MethodInfo methodInfo, AutoFaker autoFaker, params object[] args)
    {
        MethodInfo method = methodInfo.MakeGenericMethod(type);
        object? instance = method.Invoke(autoFaker, args);

        instance.Should().BeGenerated();
    }

    public static void AssertGenerateMany(Type type, MethodInfo methodInfo, AutoFaker autoFaker, params object[] args)
    {
        int count = AutoFakerDefaultConfigOptions.DefaultRepeatCount;
        MethodInfo method = methodInfo.MakeGenericMethod(type);
        var instances = method.Invoke(autoFaker, args) as ICollection;

        instances.Count.Should().Be(count);

        foreach (object? instance in instances)
        {
            instance.Should().BeGenerated();
        }
    }

    public static void AssertGenerateMany(IEnumerable<Order> instances)
    {
        int count = AutoFakerDefaultConfigOptions.DefaultRepeatCount;

        instances.Should().HaveCount(count);

        foreach (Order? instance in instances)
        {
            instance.Should().BeGeneratedWithoutMocks();
        }
    }
}