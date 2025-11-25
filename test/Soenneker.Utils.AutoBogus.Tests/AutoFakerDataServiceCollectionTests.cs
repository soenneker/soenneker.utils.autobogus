using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

/// <summary>
/// Tests for issue #884: DataServiceCollection properties cause AutoBogus to throw an exception
/// https://github.com/soenneker/soenneker.utils.autobogus/issues/884
/// </summary>
public class AutoFakerDataServiceCollectionTests
{
    [Fact]
    public void Generate_ObjectWithDataServiceCollection_should_not_throw_and_property_should_be_null()
    {
        // Arrange
        var faker = new AutoFaker();

        // Act
        // AutoBogus should handle types it doesn't understand gracefully by leaving them as null
        // instead of throwing an ArgumentException when trying to set incompatible types
        var result = faker.Generate<TestClassWithDataServiceCollection>();

        // Assert
        result.Should()
              .NotBeNull();
        // The DataServiceCollection property should be null because AutoBogus cannot convert
        // the generated List<string> to DataServiceCollection<string>
        result.Collection.Should()
              .BeNull();
    }
}