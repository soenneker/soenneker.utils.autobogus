using FluentAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.WeakReference;
using System;
using System.Reflection;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerWeakReferenceTests
{
    [Fact]
    public void Generate_TestClassWithWeakReference_should_generate()
    {
        var faker = new AutoFaker();

        var result = faker.Generate<TestClassWithWeakReference>();
        result.Should().NotBeNull();
        result.WeakReference.Should().NotBeNull();
    }

    [Fact]
    public void Generate_TestClassWithWeakReferenceT_should_generate()
    {
        var faker = new AutoFaker();

        var result = faker.Generate<TestClassWithWeakReferenceT>();
        result.Should().NotBeNull();
        result.WeakReference.TryGetTarget(out TestClassWithSimpleProperties? target).Should().BeTrue();
        target.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_TestClassWithWeakReferenceTPrivate_should_generate()
    {
        var faker = new AutoFaker();

        var result = faker.Generate<TestClassWithWeakReferenceTPrivate>();
        result.Should().NotBeNull();

        Type type = typeof(TestClassWithWeakReferenceTPrivate);

        FieldInfo? field = type.GetField("WeakReference", BindingFlags.NonPublic | BindingFlags.Instance);

        field.Should().NotBeNull();
    }

    [Fact]
    public void AutoFakerT_Generate_TestClassWithWeakReference_should_generate()
    {
        var faker = new AutoFaker<TestClassWithWeakReference>();

        var result = faker.Generate();
        result.Should().NotBeNull();
        result.WeakReference.Should().NotBeNull();
    }

    [Fact]
    public void AutoFakerT_Generate_TestClassWithWeakReferenceT_should_generate()
    {
        var faker = new AutoFaker<TestClassWithWeakReferenceT>();

        var result = faker.Generate();
        result.Should().NotBeNull();
        result.WeakReference.TryGetTarget(out TestClassWithSimpleProperties? target).Should().BeTrue();
        target.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void AutoFakerT_Generate_TestClassWithWeakReferenceTPrivate_should_generate()
    {
        var faker = new AutoFaker<TestClassWithWeakReferenceTPrivate>();

        var result = faker.Generate();
        result.Should().NotBeNull();

        Type type = typeof(TestClassWithWeakReferenceTPrivate);

        FieldInfo? field = type.GetField("WeakReference", BindingFlags.NonPublic | BindingFlags.Instance);

        field.Should().NotBeNull();
    }
}