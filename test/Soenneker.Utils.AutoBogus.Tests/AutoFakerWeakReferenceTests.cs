using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.WeakReference;
using System;
using System.Reflection;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerWeakReferenceTests
{
    [Test]
    public void Generate_TestClassWithWeakReference_should_generate()
    {
        var faker = new AutoFaker();

        var result = faker.Generate<TestClassWithWeakReference>();
        result.Should().NotBeNull();
        result.WeakReference.Should().NotBeNull();
    }

    [Test]
    public void Generate_TestClassWithWeakReferenceT_should_generate()
    {
        var faker = new AutoFaker();

        var result = faker.Generate<TestClassWithWeakReferenceT>();
        result.Should().NotBeNull();
        result.WeakReference.TryGetTarget(out TestClassWithSimpleProperties? target).Should().BeTrue();
        target.Name.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Generate_TestClassWithWeakReferenceTPrivate_should_generate()
    {
        var faker = new AutoFaker();

        var result = faker.Generate<TestClassWithWeakReferenceTPrivate>();
        result.Should().NotBeNull();

        Type type = typeof(TestClassWithWeakReferenceTPrivate);

        FieldInfo? field = type.GetField("WeakReference", BindingFlags.NonPublic | BindingFlags.Instance);

        field.Should().NotBeNull();
    }

    [Test]
    public void AutoFakerT_Generate_TestClassWithWeakReference_should_generate()
    {
        var faker = new AutoFaker<TestClassWithWeakReference>();

        TestClassWithWeakReference result = faker.Generate();
        result.Should().NotBeNull();
        result.WeakReference.Should().NotBeNull();
    }

    [Test]
    public void AutoFakerT_Generate_TestClassWithWeakReferenceT_should_generate()
    {
        var faker = new AutoFaker<TestClassWithWeakReferenceT>();

        TestClassWithWeakReferenceT result = faker.Generate();
        result.Should().NotBeNull();
        result.WeakReference.TryGetTarget(out TestClassWithSimpleProperties? target).Should().BeTrue();
        target.Name.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void AutoFakerT_Generate_TestClassWithWeakReferenceTPrivate_should_generate()
    {
        var faker = new AutoFaker<TestClassWithWeakReferenceTPrivate>();

        TestClassWithWeakReferenceTPrivate result = faker.Generate();
        result.Should().NotBeNull();

        Type type = typeof(TestClassWithWeakReferenceTPrivate);

        FieldInfo? field = type.GetField("WeakReference", BindingFlags.NonPublic | BindingFlags.Instance);

        field.Should().NotBeNull();
    }
}