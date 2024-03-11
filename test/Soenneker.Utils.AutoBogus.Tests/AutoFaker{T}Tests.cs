using FluentAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using System.Diagnostics;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;
using System.Linq;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerTTests
{
    [Fact]
    public void Generate_order_should_generate()
    {
        var autoFaker = new AutoFaker<Order>();
        autoFaker.RuleFor(x => x.Id, () => 1 );

        Order order = autoFaker.Generate();
        order.Should().NotBeNull();
        order.Id.Should().Be(1);
    }

    [Fact]
    public void Generate_record_should_generate()
    {
        var faker = new AutoFaker<TestRecord>();

        var record = faker.Generate();

        record.Should().NotBeNull();
        record.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_Product_should_generate()
    {
        var faker = new AutoFaker<Product>();

        var product = faker.Generate();
        product.Should().NotBeNull();
        product.GetRevisions.Should().NotBeNullOrEmpty();
        product.ReadOnlySet.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Generate_many_Orders_should_generate()
    {
        var faker = new AutoFaker<Order>();

        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < 1000; i++)
        {
            var order = faker.Generate();
        }

        stopwatch.Stop();
    }

    [Fact]
    public void Generate_with_default_RepeatCount_should_generate_correct_count()
    {
        var autoFaker = new AutoFaker<CustomOrder>();

        var order = autoFaker.Generate();
        order.Items.Count().Should().Be(1);
    }

    [Fact]
    public void Generate_with_set_RepeatCount_should_generate_correct_count()
    {
        var autoFaker = new AutoFaker<CustomOrder>
        {
            Config =
            {
                RepeatCount = 3
            }
        };

        var order = autoFaker.Generate();
        order.Items.Count().Should().Be(3);
    }
}