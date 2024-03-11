using System;
using Bogus;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoConfigBuilderFixture
{
    private readonly Faker _faker;
    private readonly AutoFakerConfig _fakerConfig;
    private readonly AutoFakerConfigBuilder _builder;

    private interface ITestBuilder { }

    public AutoConfigBuilderFixture()
    {
        var autoFaker = new AutoFaker();
        _faker = new Faker();
        _fakerConfig = new AutoFakerConfig();
        _builder = new AutoFakerConfigBuilder(_fakerConfig);
    }

    public class WithDateTimeKind : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_BeUniversal()
        {
            var person = AutoFaker.GenerateStatic<Human>();
            person.Birthday.Should().Be(person.Birthday.ToUniversalTime());
        }
    }

    public class WithRecursiveDepth
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Config_RecursiveDepth()
        {
            int depth = _faker.Random.Int();

            _builder.WithRecursiveDepth<ITestBuilder>( depth, null);

            _fakerConfig.RecursiveDepth.Should().Be(depth);
        }
    }

    
    public class WithTreeDepth
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Config_TreeDepth()
        {
            int depth = _faker.Random.Int();

            _builder.WithTreeDepth<ITestBuilder>(depth, null);

            _fakerConfig.TreeDepth.Should().Be(depth);
        }

        [Fact]
        public void Should_Set_Config_TreeDepth_To_Default_If_Null()
        {
            int? depth = AutoFakerDefaultConfigOptions.TreeDepth;

            _builder.WithTreeDepth<ITestBuilder>(null, null);

            _fakerConfig.TreeDepth.Should().Be(depth);
        }
    }

    public class WithSkip_Type
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Not_Add_Type_If_Already_Added()
        {
            Type type1 = typeof(int);
            Type type2 = typeof(int);

            _fakerConfig.SkipTypes =
            [
                type1
            ];

            _builder.WithSkip<ITestBuilder>(type2, null);

            _fakerConfig.SkipTypes.Should().ContainSingle();
        }

        [Fact]
        public void Should_Add_Type_To_Skip()
        {
            Type type1 = typeof(int);
            Type type2 = typeof(string);

            _fakerConfig.SkipTypes =
            [
                type1
            ];

            _builder.WithSkip<ITestBuilder>(type2, null);

            _fakerConfig.SkipTypes.Should().BeEquivalentTo(new[]
            {
                type1,
                type2
            });
        }
    }

    public class WithSkip_TypePath
        : AutoConfigBuilderFixture
    {
        private sealed class TestSkip
        {
            public string Value { get; set; }
        }

        [Fact]
        public void Should_Not_Add_Member_If_Already_Added()
        {
            Type type = typeof(TestSkip);
            var member = $"{type.FullName}.Value";

            _fakerConfig.SkipPaths =
            [
                member
            ];

            _builder.WithSkip<ITestBuilder>(type, "Value", null);

            _fakerConfig.SkipPaths.Should().ContainSingle();
        }

        [Fact]
        public void Should_Add_MemberName_To_Skip()
        {
            Type type = typeof(TestSkip);
            string? path = _faker.Random.String();

            _fakerConfig.SkipPaths =
            [
                path
            ];
            _builder.WithSkip<ITestBuilder>(type, "Value", null);

            _fakerConfig.SkipPaths.Should().BeEquivalentTo([
                path,
                $"{type.FullName}.Value"
            ]);
        }
    }

    public class WithSkip_Path
        : AutoConfigBuilderFixture
    {
        private sealed class TestSkip
        {
            public string Value { get; set; }
        }

        [Fact]
        public void Should_Not_Add_Member_If_Already_Added()
        {
            Type type = typeof(TestSkip);
            var member = $"{type.FullName}.Value";
            _fakerConfig.SkipPaths =
            [
                member
            ];

            _builder.WithSkip<ITestBuilder, TestSkip>("Value", null);

            _fakerConfig.SkipPaths.Should().ContainSingle();
        }

        [Fact]
        public void Should_Add_MemberName_To_Skip()
        {
            Type type = typeof(TestSkip);
            string? path = _faker.Random.String();

            _fakerConfig.SkipPaths =
            [
                path
            ];

            _builder.WithSkip<ITestBuilder, TestSkip>("Value", null);

            _fakerConfig.SkipPaths.Should().BeEquivalentTo([
                path,
                $"{type.FullName}.Value"
            ]);
        }
    }

    public class WithOverride
        : AutoConfigBuilderFixture
    {
        private class TestAutoFakerGeneratorOverride
            : AutoFakerGeneratorOverride
        {
            public override bool CanOverride(AutoFakerContext context)
            {
                return false;
            }

            public override void Generate(AutoFakerOverrideContext context)
            { }
        }

        [Fact]
        public void Should_Not_Add_Null_Override()
        {
            _builder.WithOverride<ITestBuilder>(null, null);

            _fakerConfig.Overrides.Should().BeNull();
        }

        [Fact]
        public void Should_Not_Add_Override_If_Already_Added()
        {
            var generatorOverride = new TestAutoFakerGeneratorOverride();

            _fakerConfig.Overrides =
            [
                generatorOverride
            ];

            _builder.WithOverride<ITestBuilder>(generatorOverride, null);

            _fakerConfig.Overrides.Should().ContainSingle();
        }

        [Fact]
        public void Should_Add_Override_If_Equivalency_Is_Different()
        {
            var generatorOverride1 = new TestAutoFakerGeneratorOverride();
            var generatorOverride2 = new TestAutoFakerGeneratorOverride();

            _fakerConfig.Overrides =
            [
                generatorOverride1
            ];

            _builder.WithOverride<ITestBuilder>(generatorOverride2, null);

            _fakerConfig.Overrides.Should().BeEquivalentTo(new[]
            {
                generatorOverride1,
                generatorOverride2
            });
        }
    }

    public class WithArgs
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Args()
        {
            var args = new object[] { _faker.Random.Int(), _faker.Random.String() };

            _builder.WithArgs<ITestBuilder>(args, null);

            _builder.Args.Should().BeSameAs(args);
        }
    }
}