using System;
using FluentAssertions;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoGeneratorOverridesFixture
{
    private sealed class TestOverride
        : AutoFakerGeneratorOverride
    {
        public TestOverride(bool preinitialize, Action<AutoFakerOverrideContext> generator)
        {
            Preinitialize = preinitialize;
            Generator = generator;
        }

        public override bool Preinitialize { get; }
        private Action<AutoFakerOverrideContext> Generator { get; }

        public override bool CanOverride(AutoFakerContext context)
        {
            return context.GenerateType == typeof(OverrideClass);
        }

        public override void Generate(AutoFakerOverrideContext context)
        {
            Generator.Invoke(context);
        }
    }

    [Fact]
    public void Should_Initialize_As_Configured()
    {
        AutoFaker.Generate<OverrideClass>(builder =>
        {
            builder
                .WithOverride(new TestOverride(false, context => context.Instance.Should().BeNull()))
                .WithOverride(new TestOverride(true, context => context.Instance.Should().NotBeNull()))
                .WithOverride(new TestOverride(false, context => context.Instance.Should().NotBeNull()));
        });
    }

    // [Fact]
    // public void Should_Invoke_Type_Override()
    // {
    //     var value = AutoFaker.Generate<int>();
    //     var result = AutoFaker.Generate<OverrideClass>(builder =>
    //     {
    //         builder.WithOverride(context =>
    //         {
    //             var instance = context.Instance as OverrideClass;
    //             var method = typeof(OverrideId).GetMethod("SetValue");
    //
    //             method.Invoke(instance.Id, new object[] { value });
    //
    //             return instance;
    //         });
    //     });
    //
    //     result.Id.Value.Should().Be(value);
    //     result.Name.Should().NotBeNull();
    //     result.Amounts.Should().NotBeEmpty();
    // }
}