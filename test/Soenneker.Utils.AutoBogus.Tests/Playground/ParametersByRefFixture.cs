using AwesomeAssertions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Playground;

public class ParametersByRefFixture
{
    private class TestId
    {
        public string Value { get; set; }
    }

    private sealed class TestClass
    {
        public TestClass(in TestId id, out string value)
        {
            value = "OUT";

            Id = id;
            Value = value;
        }

        public TestId Id { get; }
        public string Value { get; }
    }

    [Fact]
    public void Should_Create_Instance()
    {
        var instance = AutoFaker.GenerateStatic<TestClass>();
        instance.Should().NotBeNull();
    }
}