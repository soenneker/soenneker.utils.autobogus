using FluentAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.CalendarItem;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Enums;

public class AutoFakerIntellenumTests
{
    [Fact]
    public void Generate_calendar_should_generate()
    {
        var faker = new AutoFaker();

        var calendar = faker.Generate<CalendarItemIntellenum>();
        calendar.Should().NotBeNull();

        calendar.DayOfWeek.Value.Should().BeOneOf("Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday");
    }
}