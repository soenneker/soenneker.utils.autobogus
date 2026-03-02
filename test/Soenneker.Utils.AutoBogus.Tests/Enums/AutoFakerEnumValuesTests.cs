using System.Linq;
using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.CalendarItem;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Enums;

public class AutoFakerEnumValuesTests
{
    [Fact]
    public void Generate_CalendarItemEnumValues_should_generate()
    {
        var faker = new AutoFaker();

        var item = faker.Generate<CalendarItemEnumValues>();

        item.Should().NotBeNull();
        item.Status.Should().NotBeNull();
        item.Status.Value.Should().BeOneOf(1, 2, 3);
    }

    [Fact]
    public void Generate_OrderStatusEnumValue_directly_should_return_one_of_defined_values()
    {
        var faker = new AutoFaker();

        var value = faker.Generate<OrderStatusEnumValue>();

        value.Should().NotBeNull();
        value.Should().BeOneOf(
            OrderStatusEnumValue.Pending,
            OrderStatusEnumValue.Completed,
            OrderStatusEnumValue.Cancelled);
    }

    [Fact]
    public void Generate_many_CalendarItemEnumValues_should_produce_varied_statuses()
    {
        var faker = new AutoFaker();
        var items = faker.Generate<CalendarItemEnumValues>(100);

        var distinctValues = items.Select(i => i.Status.Value).Distinct().ToList();

        distinctValues.Should().HaveCountGreaterThan(1);
        distinctValues.Should().OnlyContain(v => v == 1 || v == 2 || v == 3);
    }
}
