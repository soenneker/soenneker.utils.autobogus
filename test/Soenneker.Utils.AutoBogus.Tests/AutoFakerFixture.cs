using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bogus;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Services;
using Soenneker.Utils.AutoBogus.Tests.Dtos;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerFixture
{
    private class TestFakerBinder
        : AutoFakerBinder
    { }

    private const string _name = "Generate";
    private static readonly Type _type = typeof(AutoFaker);

    public class Configure
        : AutoFakerFixture
    {
        [Fact]
        public void Should_Configure_Default_Config()
        {
            AutoFakerConfig fakerConfig = null;
            AutoFaker.Configure(builder =>
            {
                var instance = builder as AutoFakerConfigBuilder;
                fakerConfig = instance.FakerConfig;
            });

            fakerConfig.Should().Be(DefaultConfigService.Config);
        }
    }

    public class Create
        : AutoFakerFixture
    {
        [Fact]
        public void Should_Configure_Child_Config()
        {
            Action<IAutoGenerateConfigBuilder>? configure = CreateConfigure<IAutoGenerateConfigBuilder>(DefaultConfigService.Config);
            AutoFaker.Create(configure).Should().BeOfType<AutoFaker>();
        }
    }

    public class Generate_Instance
        : AutoFakerFixture
    {
        private static Type _interfaceType = typeof(IAutoFaker);
        private static string _methodName = $"{_interfaceType.FullName}.{_name}";
        private static MethodInfo _generate = _type.GetMethod(_methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Action<IAutoGenerateConfigBuilder>) }, null);
        private static MethodInfo _generateMany = _type.GetMethod(_methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(int), typeof(Action<IAutoGenerateConfigBuilder>) }, null);

        private IAutoFaker _faker;
        private AutoFakerConfig _fakerConfig;

        public Generate_Instance()
        {
            var faker = AutoFaker.Create() as AutoFaker;

            _faker = faker;
            _fakerConfig = faker.FakerConfig;
        }

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Should_Generate_Type(Type type)
        {
            Action<IAutoGenerateConfigBuilder>? configure = CreateConfigure<IAutoGenerateConfigBuilder>(_fakerConfig);
            AssertGenerate(type, _generate, _faker, configure);
        }

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Should_Generate_Many_Types(Type type)
        {
            int count = AutoFakerConfig.DefaultRepeatCount.Invoke(null);
            Action<IAutoGenerateConfigBuilder>? configure = CreateConfigure<IAutoGenerateConfigBuilder>(_fakerConfig);

            AssertGenerateMany(type, _generateMany, _faker, count, configure);
        }

        [Fact]
        public void Should_Generate_Complex_Type()
        {
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(_fakerConfig);
            _faker.Generate<Order>(configure).Should().BeGeneratedWithoutMocks();
        }

        [Fact]
        public void Should_Generate_Many_Complex_Types()
        {
            int count = AutoFakerConfig.DefaultRepeatCount.Invoke(null);
            Action<IAutoGenerateConfigBuilder>? configure = CreateConfigure<IAutoGenerateConfigBuilder>(_fakerConfig);
            List<Order>? instances = _faker.Generate<Order>(count, configure);

            AssertGenerateMany(instances);
        }
    }

    public class Generate_Instance_Faker : AutoFakerFixture
    {
        private readonly IAutoFaker _faker;
        private AutoFakerConfig _fakerConfig;

        public Generate_Instance_Faker()
        {
            var faker = AutoFaker.Create() as AutoFaker;

            _faker = faker;
            _fakerConfig = faker.FakerConfig;
        }
    }

    public class Generate_Static
        : AutoFakerFixture
    {
        private static readonly MethodInfo _generate = _type.GetMethod(_name, BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Action<IAutoGenerateConfigBuilder>) }, null);
        private static readonly MethodInfo _generateMany = _type.GetMethod(_name, BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(int), typeof(Action<IAutoGenerateConfigBuilder>) }, null);

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Should_Generate_Type(Type type)
        {
            Action<IAutoGenerateConfigBuilder>? configure = CreateConfigure<IAutoGenerateConfigBuilder>(DefaultConfigService.Config);
            AssertGenerate(type, _generate, null, configure);
        }

        [Theory]
        [MemberData(nameof(GetTypes))]
        public void Should_Generate_Many_Types(Type type)
        {
            int count = AutoFakerConfig.DefaultRepeatCount.Invoke(null);
            Action<IAutoGenerateConfigBuilder>? configure = CreateConfigure<IAutoGenerateConfigBuilder>(DefaultConfigService.Config);

            AssertGenerateMany(type, _generateMany, null, count, configure);
        }

        [Fact]
        public void Should_Generate_Complex_Type()
        {
            Action<IAutoGenerateConfigBuilder>? configure = CreateConfigure<IAutoGenerateConfigBuilder>(DefaultConfigService.Config);
            AutoFaker.Generate<Order>(configure).Should().BeGeneratedWithoutMocks();
        }

        [Fact]
        public void Should_Generate_Many_Complex_Types()
        {
            int count = AutoFakerConfig.DefaultRepeatCount.Invoke(null);
            Action<IAutoGenerateConfigBuilder>? configure = CreateConfigure<IAutoGenerateConfigBuilder>(DefaultConfigService.Config);
            List<Order>? instances = AutoFaker.Generate<Order>(count, configure);

            AssertGenerateMany(instances);
        }
    }

    public class AutoFaker_WithFakerHub : AutoFakerFixture
    {
        [Fact]
        public void Should_Use_Caller_Supplied_Faker()
        {
            // We infer that our FakerHub was used by reseeding it and testing that we get the same sequence both times.
            var faker = new Faker();

            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>(
                DefaultConfigService.Config,
                builder => builder.WithFaker(faker));

            IAutoFaker autoFaker = AutoFaker.Create(configure);

            faker.Random = new Randomizer(1);

            var instance1 = autoFaker.Generate<TestObject>();

            faker.Random = new Randomizer(1);

            var instance2 = autoFaker.Generate<TestObject>();

            instance1.Should().BeEquivalentTo(instance2);
        }

        class TestObject
        {
            public int IntegerValue;
            public string StringValue;
        }
    }

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
            AutoFaker.Generate<ITestInterface>().Should().BeNull();
        }

        [Fact]
        public void Should_Not_Generate_Abstract_Class_Type()
        {
            AutoFaker.Generate<TestAbstractClass>().Should().BeNull();
        }
    }

    public class Behaviors_Recursive
        : AutoFakerFixture
    {
        private TestRecursiveClass _instance;

        public Behaviors_Recursive()
        {
            _instance = AutoFaker.Generate<TestRecursiveClass>(builder =>
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
            IEnumerable<TestRecursiveClass>? children = _instance.Children;
            List<TestRecursiveClass>? children1 = children.SelectMany(c => c.Children).ToList();
            List<TestRecursiveClass>? children2 = children1.SelectMany(c => c.Children).ToList();
            List<TestRecursiveClass>? children3 = children2.Where(c => c.Children != null).ToList();

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
            yield return new object[] { type };
        }

        yield return new object[] { typeof(string[]) };
        yield return new object[] { typeof(TestEnum) };
        yield return new object[] { typeof(IDictionary<Guid, TestStruct>) };
        yield return new object[] { typeof(IEnumerable<TestClass>) };
        yield return new object[] { typeof(int?) };
    }

    private static Action<TBuilder> CreateConfigure<TBuilder>(AutoFakerConfig assertFakerConfig, Action<TBuilder> configure = null)
    {
        return builder =>
        {
            if (configure != null)
            {
                configure.Invoke(builder);
            }

            var instance = builder as AutoFakerConfigBuilder;
            instance.FakerConfig.Should().NotBe(assertFakerConfig);
        };
    }

    public static void AssertGenerate(Type type, MethodInfo methodInfo, IAutoFaker faker, params object[] args)
    {
        MethodInfo? method = methodInfo.MakeGenericMethod(type);
        object? instance = method.Invoke(faker, args);

        instance.Should().BeGenerated();
    }

    public static void AssertGenerateMany(Type type, MethodInfo methodInfo, IAutoFaker faker, params object[] args)
    {
        int count = AutoFakerConfig.DefaultRepeatCount.Invoke(null);
        MethodInfo? method = methodInfo.MakeGenericMethod(type);
        var instances = method.Invoke(faker, args) as ICollection;

        instances.Count.Should().Be(count);

        foreach (object? instance in instances)
        {
            instance.Should().BeGenerated();
        }
    }

    public static void AssertGenerateMany(IEnumerable<Order> instances)
    {
        int count = AutoFakerConfig.DefaultRepeatCount.Invoke(null);

        instances.Should().HaveCount(count);

        foreach (Order? instance in instances)
        {
            instance.Should().BeGeneratedWithoutMocks();
        }
    }
}