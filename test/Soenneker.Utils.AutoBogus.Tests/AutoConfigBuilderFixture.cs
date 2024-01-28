using System;
using Bogus;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
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
        _faker = new Faker();
        _fakerConfig = new AutoFakerConfig();
        _builder = new AutoFakerConfigBuilder(_fakerConfig);
    }

    public class WithLocale
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Config_Locale()
        {
            string? locale = _faker.Random.String();

            _builder.WithLocale<ITestBuilder>(locale, null);

            _fakerConfig.Locale.Should().Be(locale);
        }

        [Fact]
        public void Should_Set_Config_Locale_To_Default_If_Null()
        {
            _fakerConfig.Locale = _faker.Random.String();

            _builder.WithLocale<ITestBuilder>(null, null);

            _fakerConfig.Locale.Should().Be(AutoFakerConfig.DefaultLocale);
        }
    }

    public class WithDateTimeKind : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Config_DateTimeKind()
        {
            var kind = DateTimeKind.Utc;
            _builder.WithDateTimeKind<ITestBuilder>(context => kind, null);
            _fakerConfig.DateTimeKind.Invoke(null).Should().Be(kind);
        }

        [Fact]
        public void Should_Set_Config_DateTimeKind_To_Default_If_Null()
        {
            var kind = AutoFakerConfig.DefaultDateTimeKind.Invoke(null);
            _builder.WithDateTimeKind<ITestBuilder>(null, null);
            _fakerConfig.DateTimeKind.Invoke(null).Should().Be(kind);
        }

        private sealed record Obj(DateTime Birthday);

        [Fact]
        public void Should_ConvertToUtc()
        {
            var obj = AutoFaker.Generate<Obj>(builder =>
            {
                builder.WithDateTimeKind(DateTimeKind.Utc);
            });
            obj.Birthday.Should().Be(obj.Birthday.ToUniversalTime());
        }

        [Fact]
        public void Should_BeLocal()
        {
            var obj = AutoFaker.Generate<Obj>();
            obj.Birthday.Should().Be(obj.Birthday.ToLocalTime());
        }
    }

    public class WithRepeatCount
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Config_RepeatCount()
        {
            int count = _faker.Random.Int();

            _builder.WithRepeatCount<ITestBuilder>(context => count, null);

            _fakerConfig.RepeatCount.Invoke(null).Should().Be(count);
        }

        [Fact]
        public void Should_Set_Config_RepeatCount_To_Default_If_Null()
        {
            int count = AutoFakerConfig.DefaultRepeatCount.Invoke(null);

            _builder.WithRepeatCount<ITestBuilder>(null, null);

            _fakerConfig.RepeatCount.Invoke(null).Should().Be(count);
        }
    }

    public class WithRecursiveDepth
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Config_RecursiveDepth()
        {
            int depth = _faker.Random.Int();

            _builder.WithRecursiveDepth<ITestBuilder>(context => depth, null);

            _fakerConfig.RecursiveDepth.Invoke(null).Should().Be(depth);
        }

        [Fact]
        public void Should_Set_Config_RecursiveDepth_To_Default_If_Null()
        {
            int depth = AutoFakerConfig.DefaultRecursiveDepth.Invoke(null);

            _builder.WithRecursiveDepth<ITestBuilder>(null, null);

            _fakerConfig.RecursiveDepth.Invoke(null).Should().Be(depth);
        }
    }

    
    public class WithTreeDepth
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Config_TreeDepth()
        {
            int depth = _faker.Random.Int();

            _builder.WithTreeDepth<ITestBuilder>(context => depth, null);

            _fakerConfig.TreeDepth.Invoke(null).Should().Be(depth);
        }

        [Fact]
        public void Should_Set_Config_TreeDepth_To_Default_If_Null()
        {
            int? depth = AutoFakerConfig.DefaultTreeDepth.Invoke(null);

            _builder.WithTreeDepth<ITestBuilder>(null, null);

            _fakerConfig.TreeDepth.Invoke(null).Should().Be(depth);
        }
    }
    
    public class WithFakerHub
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Set_Config_FakerHub()
        {
            var faker = new Faker();

            _builder.WithFaker<ITestBuilder>(faker, null);
            _builder.FakerConfig.Faker.Should().Be(faker);
        }
    }

    public class WithSkip_Type
        : AutoConfigBuilderFixture
    {
        [Fact]
        public void Should_Not_Add_Type_If_Already_Added()
        {
            Type? type1 = typeof(int);
            Type? type2 = typeof(int);

            _fakerConfig.SkipTypes.Add(type1);

            _builder.WithSkip<ITestBuilder>(type2, null);

            _fakerConfig.SkipTypes.Should().ContainSingle();
        }

        [Fact]
        public void Should_Add_Type_To_Skip()
        {
            Type? type1 = typeof(int);
            Type? type2 = typeof(string);

            _fakerConfig.SkipTypes.Add(type1);

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
            Type? type = typeof(TestSkip);
            var member = $"{type.FullName}.Value";

            _fakerConfig.SkipPaths.Add(member);

            _builder.WithSkip<ITestBuilder>(type, "Value", null);

            _fakerConfig.SkipPaths.Should().ContainSingle();
        }

        [Fact]
        public void Should_Add_MemberName_To_Skip()
        {
            Type? type = typeof(TestSkip);
            string? path = _faker.Random.String();

            _fakerConfig.SkipPaths.Add(path);

            _builder.WithSkip<ITestBuilder>(type, "Value", null);

            _fakerConfig.SkipPaths.Should().BeEquivalentTo(new[]
            {
                path,
                $"{type.FullName}.Value"
            });
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
            Type? type = typeof(TestSkip);
            var member = $"{type.FullName}.Value";

            _fakerConfig.SkipPaths.Add(member);

            _builder.WithSkip<ITestBuilder, TestSkip>("Value", null);

            _fakerConfig.SkipPaths.Should().ContainSingle();
        }

        [Fact]
        public void Should_Add_MemberName_To_Skip()
        {
            Type? type = typeof(TestSkip);
            string? path = _faker.Random.String();

            _fakerConfig.SkipPaths.Add(path);

            _builder.WithSkip<ITestBuilder, TestSkip>("Value", null);

            _fakerConfig.SkipPaths.Should().BeEquivalentTo(new[]
            {
                path,
                $"{type.FullName}.Value"
            });
        }
    }

    public class WithOverride
        : AutoConfigBuilderFixture
    {
        private class TestGeneratorOverride
            : GeneratorOverride
        {
            public override bool CanOverride(AutoFakerContext context)
            {
                return false;
            }

            public override void Generate(AutoFakerContextOverride context)
            { }
        }

        [Fact]
        public void Should_Not_Add_Null_Override()
        {
            _builder.WithOverride<ITestBuilder>(null, null);

            _fakerConfig.Overrides.Should().BeEmpty();
        }

        [Fact]
        public void Should_Not_Add_Override_If_Already_Added()
        {
            var generatorOverride = new TestGeneratorOverride();
            _fakerConfig.Overrides.Add(generatorOverride);

            _builder.WithOverride<ITestBuilder>(generatorOverride, null);

            _fakerConfig.Overrides.Should().ContainSingle();
        }

        [Fact]
        public void Should_Add_Override_If_Equivalency_Is_Different()
        {
            var generatorOverride1 = new TestGeneratorOverride();
            var generatorOverride2 = new TestGeneratorOverride();

            _fakerConfig.Overrides.Add(generatorOverride1);

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