using System;
using FluentAssertions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Types;

public class TimeSpanTests
{
    [Fact]
    public void TimeSpan_should_be_generated()
    {
        var faker = new AutoFaker();
        var obj = faker.Generate<TimeSpan>();
        obj.Seconds.Should().BeGreaterThan(1);
        obj.GetType().Should().Be(typeof(TimeSpan));
    }
}