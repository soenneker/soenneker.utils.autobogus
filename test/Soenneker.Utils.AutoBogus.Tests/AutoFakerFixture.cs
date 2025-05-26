using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Extensions;
using Soenneker.Utils.AutoBogus.Tests.TestData;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerFixture
{
    private static readonly Type _type = typeof(AutoFaker);

    public class Configure : AutoFakerFixture
    {
        [Fact]
        public void Should_Configure_Default_Config()
        {
            AutoFakerConfig? fakerConfig = null;

            var autoFaker = new AutoFaker();

            autoFaker.Configure(builder =>
            {
                var instance = builder as AutoFakerConfigBuilder;
                fakerConfig = instance.AutoFakerConfig;
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
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>();

            var autoFaker = new AutoFaker(configure);
            autoFaker.Should().BeOfType<AutoFaker>();
        }
    }

    public class Generate : AutoFakerFixture
    {
        private static readonly MethodInfo _generateMany =
            _type.GetMethod("GenerateStatic", BindingFlags.Static | BindingFlags.Public, null, [typeof(int), typeof(Action<IAutoGenerateConfigBuilder>)], null);

        private readonly AutoFaker autoFaker = new AutoFaker();

        [Theory]
        [ClassData(typeof(TypeTestData))]
        public void Should_Generate_Type(Type type, Type? expectedType)
        {
            object result = autoFaker.Generate(type);

            result.GetType().Should().Be(expectedType);
        }

        [Theory]
        [ClassData(typeof(TypeTestData))]
        public void Should_Generate_Many_Types(Type type, Type expectedType)
        {
            int count = AutoFakerDefaultConfigOptions.RepeatCount;
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>();

            AssertGenerateMany(type, _generateMany, null, count, configure);
        }

        [Fact]
        public void Should_Generate_Complex_Type()
        {
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>();
            AutoFaker.GenerateStatic<Order>(configure).Should().BeGeneratedWithoutMocks();
        }

        [Fact]
        public void Should_Generate_Many_Complex_Types()
        {
            int count = AutoFakerDefaultConfigOptions.RepeatCount;
            Action<IAutoGenerateConfigBuilder> configure = CreateConfigure<IAutoGenerateConfigBuilder>();
            List<Order> instances = AutoFaker.GenerateStatic<Order>(count, configure);

            AssertGenerateMany(instances);
        }
    }

    public class Behaviors_Types : AutoFakerFixture
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

    public class Behaviors_Recursive : AutoFakerFixture
    {
        private readonly TestRecursiveClass _instance;

        readonly AutoFaker _autoFaker;

        public Behaviors_Recursive()
        {
            var config = new AutoFakerConfig();
            config.RecursiveDepth = 3;
            config.RepeatCount = 3;

            _autoFaker = new AutoFaker(config);
        }

        [Fact]
        public void Should_Generate_Recursive_Types()
        {
            var instance = _autoFaker.Generate<TestRecursiveClass>();

            instance.Child.Should().NotBeNull();
            instance.Child.Child.Should().NotBeNull();
            instance.Child.Child.Child.Child.Should().BeNull();
        }

        [Fact]
        public void Should_Generate_Recursive_Lists()
        {
            var instance = _autoFaker.Generate<TestRecursiveClass>();

            List<TestRecursiveClass> children1 = instance.Children.SelectMany(c => c.Children).ToList();
            List<TestRecursiveClass> children2 = children1.SelectMany(c => c.Children).ToList();
            List<TestRecursiveClass> children3 = children2.Where(c => c.Children != null).ToList();

            instance.Children.Should().HaveCount(3);
            children1.Should().HaveCount(9);
            children2.Should().HaveCount(27);
            children3.Should().HaveCount(0);
        }

        [Fact]
        public void Should_Generate_Recursive_Sub_Types()
        {
            var instance = _autoFaker.Generate<TestRecursiveClass>();

            instance.Sub.Should().NotBeNull();
            instance.Sub.Value.Sub.Should().NotBeNull();
            instance.Sub.Value.Sub.Value.Sub.Should().NotBeNull();
            instance.Sub.Value.Sub.Value.Sub.Value.Sub.Should().BeNull();
        }
    }

    private static Action<TBuilder> CreateConfigure<TBuilder>(Action<TBuilder>? configure = null)
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
        int count = AutoFakerDefaultConfigOptions.RepeatCount;
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
        int count = AutoFakerDefaultConfigOptions.RepeatCount;

        instances.Should().HaveCount(count);

        foreach (Order? instance in instances)
        {
            instance.Should().BeGeneratedWithoutMocks();
        }
    }
}