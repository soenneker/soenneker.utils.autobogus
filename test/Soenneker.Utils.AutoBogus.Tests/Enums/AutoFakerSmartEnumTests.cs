using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.CalendarItem;

namespace Soenneker.Utils.AutoBogus.Tests.Enums;

public class AutoFakerSmartEnumTests
{
    [Test]
    public void Generate_CalendarItemSmartEnum_should_generate()
    {
        var faker = new AutoFaker();

        var calendar = faker.Generate<CalendarItemSmartEnum>();
        calendar.Should().NotBeNull();

        calendar.DayOfWeek.Name.Should().BeOneOf("Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday");
    }

    [Test]
    public void Generate_CalendarItemDerivedSmartEnum_should_generate()
    {
        var faker = new AutoFaker();

        var calendar = faker.Generate<CalendarItemDerivedSmartEnum>();
        calendar.Should().NotBeNull();



        calendar.DayOfWeek.Name.Should().BeOneOf("Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday");
    }
}