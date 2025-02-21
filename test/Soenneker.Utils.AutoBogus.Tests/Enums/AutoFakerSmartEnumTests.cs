using FluentAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Enums;

public class AutoFakerSmartEnumTests
{
    [Fact]
    public void Generate_calendar_should_generate()
    {
        var faker = new AutoFaker();

        var calendar = faker.Generate<CalendarItemSmartEnum>();
        calendar.Should().NotBeNull();

        calendar.DayOfWeek.Name.Should().BeOneOf("Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday");
    }
}