using System;
using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Tests.Dtos;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

/// <summary>
/// Tests to verify that SkipPaths functionality works correctly with parent type names
/// instead of member type names.
/// </summary>
public class AutoFakerSkipPathsTests
{
    [Fact]
    public void SkipPaths_Should_Use_Parent_Type_Name_Not_Member_Type_Name()
    {
        // Arrange
        var config = new AutoFakerConfig
        {
            SkipPaths =
            [
                $"{typeof(TestSkipClass).FullName}.StringProperty",

                $"{typeof(TestSkipClass).FullName}.IntProperty"
            ]
        };

        var autoFaker = new AutoFaker<TestSkipClass>(config);

        // Act
        TestSkipClass instance = autoFaker.Generate();

        // Assert
        instance.Should().NotBeNull();
        
        // Skipped properties should be default values (null for string, 0 for int)
        instance.StringProperty.Should().BeNull("StringProperty should be skipped and remain null");
        instance.IntProperty.Should().Be(0, "IntProperty should be skipped and remain 0");
        
        // Non-skipped properties should be generated
        instance.DateTimeProperty.Should().NotBe(default(DateTime), "DateTimeProperty should be generated");
        // BoolProperty is a bool, so it should be either true or false (not default)
        instance.DecimalProperty.Should().NotBe(0, "DecimalProperty should be generated");
    }

    [Fact]
    public void SkipPaths_Should_Not_Work_With_Member_Type_Name()
    {
        // Arrange - Using member type names (this should NOT work after the fix)
        var config = new AutoFakerConfig
        {
            SkipPaths =
            [
                "System.String.StringProperty",

                "System.Int32.IntProperty"
            ]
        };

        var autoFaker = new AutoFaker<TestSkipClass>(config);

        // Act
        TestSkipClass instance = autoFaker.Generate();

        // Assert
        instance.Should().NotBeNull();
        
        // Properties should NOT be skipped because we used wrong type names
        instance.StringProperty.Should().NotBeNull("StringProperty should be generated because skip path used wrong type");
        instance.IntProperty.Should().NotBe(0, "IntProperty should be generated because skip path used wrong type");
    }

    [Fact]
    public void SkipPaths_Should_Work_With_Multiple_Properties_Of_Same_Member_Type()
    {
        // Arrange - Test that multiple string properties can be skipped individually
        var config = new AutoFakerConfig
        {
            SkipPaths = [$"{typeof(TestSkipClass).FullName}.StringProperty"]
        };

        var autoFaker = new AutoFaker<TestSkipClass>(config);

        // Act
        TestSkipClass instance = autoFaker.Generate();

        // Assert
        instance.Should().NotBeNull();
        
        // Only StringProperty should be skipped
        instance.StringProperty.Should().BeNull("StringProperty should be skipped");
        instance.IntProperty.Should().NotBe(0, "IntProperty should be generated");
        instance.DateTimeProperty.Should().NotBe(default(DateTime), "DateTimeProperty should be generated");
    }

    [Fact]
    public void SkipPaths_Should_Work_With_All_Properties()
    {
        // Arrange - Skip all properties
        var config = new AutoFakerConfig
        {
            SkipPaths =
            [
                $"{typeof(TestSkipClass).FullName}.StringProperty",

                $"{typeof(TestSkipClass).FullName}.IntProperty",

                $"{typeof(TestSkipClass).FullName}.DateTimeProperty",

                $"{typeof(TestSkipClass).FullName}.BoolProperty",

                $"{typeof(TestSkipClass).FullName}.DecimalProperty"
            ]
        };

        var autoFaker = new AutoFaker<TestSkipClass>(config);

        // Act
        TestSkipClass instance = autoFaker.Generate();

        // Assert
        instance.Should().NotBeNull();
        
        // All properties should be default values
        instance.StringProperty.Should().BeNull("StringProperty should be skipped");
        instance.IntProperty.Should().Be(0, "IntProperty should be skipped");
        instance.DateTimeProperty.Should().Be(default(DateTime), "DateTimeProperty should be skipped");
        instance.BoolProperty.Should().BeFalse("BoolProperty should be skipped");
        instance.DecimalProperty.Should().Be(0, "DecimalProperty should be skipped");
    }

    /// <summary>
    /// Test class with nested types to ensure the fix works with complex scenarios
    /// </summary>
    private sealed class TestNestedSkipClass
    {
        public TestSkipClass NestedObject { get; set; }
        public string DirectProperty { get; set; }
    }

    [Fact]
    public void SkipPaths_Should_Work_With_Nested_Objects()
    {
        // Arrange
        var config = new AutoFakerConfig
        {
            SkipPaths = [$"{typeof(TestNestedSkipClass).FullName}.DirectProperty"]
        };

        var autoFaker = new AutoFaker<TestNestedSkipClass>(config);

        // Act
        TestNestedSkipClass instance = autoFaker.Generate();

        // Assert
        instance.Should().NotBeNull();
        
        // Direct property should be skipped
        instance.DirectProperty.Should().BeNull("DirectProperty should be skipped");
        
        // Nested object should be generated normally (its properties are not skipped)
        instance.NestedObject.Should().NotBeNull("NestedObject should be generated");
        instance.NestedObject.StringProperty.Should().NotBeNull("Nested object's StringProperty should be generated");
    }
}
