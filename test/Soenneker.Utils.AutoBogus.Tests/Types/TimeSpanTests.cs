﻿using System;
using AwesomeAssertions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Types;

public class TimeSpanTests
{
    [Fact]
    public void TimeSpan_should_be_generated()
    {
        var faker = new AutoFaker();
        var obj = faker.Generate<TimeSpan>();
        obj.TotalSeconds.Should().BeGreaterThan(1);
        obj.GetType().Should().Be(typeof(TimeSpan));
    }
}