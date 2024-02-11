using FluentAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using System.Collections.Generic;
using System.Diagnostics;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;
using Soenneker.Utils.AutoBogus.Tests.Overrides;
using System.Linq;
using Soenneker.Utils.AutoBogus.Config;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerTests
{
    [Fact]
    public void Generate_order_should_generate()
    {
        var faker = new AutoFaker();

        var order = faker.Generate<Order>();
        order.Should().NotBeNull();
    }


    [Fact]
    public void Generate_dictionary_should_generate()
    {
        var faker = new AutoFaker();

        var dictionary = faker.Generate<Dictionary<int, string>>();
        dictionary.Should().NotBeNull();
    }

    [Fact]
    public void Generate_struct_should_generate()
    {
        var faker = new AutoFaker();

        var structObj = faker.Generate<TestStruct>();
        structObj.Should().NotBeNull();

        // product.Reviews.Count.Should().BeGreaterThan(0);
        // product.
    }

    [Fact]
    public void Generate_Product_should_generate()
    {
        var faker = new AutoFaker();

        var product = faker.Generate<Product>();
        product.Should().NotBeNull();
        product.GetRevisions.Should().NotBeNullOrEmpty();
        // product.Reviews.Count.Should().BeGreaterThan(0);
        // product.
    }

    [Fact]
    public void Generate_many_Orders_should_generate()
    {
        var faker = new AutoFaker();

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < 1000; i++)
        {
            var order = faker.Generate<Order>();
        }

        stopwatch.Stop();
    }

    [Fact]
    public void Generate_many_int_should_generate()
    {
        var faker = new AutoFaker();

        List<int> intList = new List<int>();

        for (var i = 0; i < 1000; i++)
        {
            var generated = faker.Generate<int>();
            intList.Add(generated);
        }

        intList.Count.Should().Be(1000);
    }

    [Fact]
    public void Generate_with_override_should_give_result()
    {
        for (var i = 0; i < 100; i++)
        {
            var config = new AutoFakerConfig
            {
                Overrides =
                [
                    new BaseCustomOrderOverride(),
                    new LongitudeOverride(),
                    new CustomOrderOverride()
                ]
            };

            var autoFaker = new AutoFaker(config);

            var order = autoFaker.Generate<CustomOrder>();
            order.CustomId.Should().Be("Blah");
        }
    }

    [Fact]
    public void Generate_with_default_RepeatCount_should_generate_correct_count()
    {
        var autoFaker = new AutoFaker();

        var order = autoFaker.Generate<CustomOrder>();
        order.Items.Count().Should().Be(1);
    }

    [Fact]
    public void Generate_with_set_RepeatCount_should_generate_correct_count()
    {
        var autoFaker = new AutoFaker();
        autoFaker.Config.RepeatCount = 3;

        var order = autoFaker.Generate<CustomOrder>();
        order.Items.Count().Should().Be(3);
    }

    [Fact]
    public void Generate_with_smartenum_should_generate()
    {
        var config = new AutoFakerConfig
        {
            Overrides =
            [
                new BaseCustomOrderOverride(),
                new LongitudeOverride(),
                new CustomOrderOverride()
            ]
        };

        var autoFaker = new AutoFaker(config);

        var order = autoFaker.Generate<CustomOrder>();
        order.DaysOfWeek.Should().NotBeNull();
        order.NullableDaysOfWeek.Should().NotBeNull();
        order.Longitude.Should().NotBeNull();
        CustomOrder.Constant.Should().Be("Order2x89ei");
    }
}