using System.Collections.Immutable;
using System.Collections.ObjectModel;
using AwesomeAssertions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Playground;

public class ImmutableDictionaryFixture
{
    private class Obj
    {
        public string SomeStringProperty { get; set; }
        public ReadOnlyDictionary<string, string> SomeReadOnlyDictionary { get; set; }
        public ImmutableDictionary<string, string> SomeImmutableDictionary { get; set; }
    }

    [Fact]
    public void Should_Populate_Object()
    {
        var obj = AutoFaker.GenerateStatic<Obj>();

        obj.Should().NotBeNull();
        obj.SomeStringProperty.Should().NotBeNullOrWhiteSpace();
        obj.SomeReadOnlyDictionary.Should().NotBeEmpty();
        obj.SomeImmutableDictionary.Should().BeNull();
    }
}