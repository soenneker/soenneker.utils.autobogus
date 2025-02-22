using FluentAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.CalendarItem;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Enums;

public class AutoFakerSmartEnumTests
{
    [Fact]
    public void Generate_CalendarItemSmartEnum_should_generate()
    {
        var faker = new AutoFaker();

        var calendar = faker.Generate<CalendarItemSmartEnum>();
        calendar.Should().NotBeNull();

        calendar.DayOfWeek.Name.Should().BeOneOf("Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday");
    }

    [Fact]
    public void Generate_CalendarItemDerivedSmartEnum_should_generate()
    {
        var faker = new AutoFaker();

        var calendar = faker.Generate<CalendarItemDerivedSmartEnum>();
        calendar.Should().NotBeNull();



        calendar.DayOfWeek.Name.Should().BeOneOf("Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday");
    }
}