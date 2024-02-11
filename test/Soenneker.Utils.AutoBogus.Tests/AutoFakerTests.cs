using FluentAssertions;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using System.Collections.Generic;
using System.Diagnostics;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;
using Xunit;
using Soenneker.Utils.AutoBogus.Tests.Overrides;

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
    public void ConfigureOverride_after_initialization_should_succeed()
    {
        var autoFaker = new AutoFaker();
        autoFaker.Config.TreeDepth = 1;

        SetOverride(autoFaker);

        var order = autoFaker.Generate<CustomOrder>();
        order.CustomId.Should().Be("Blah");
    }

    [Fact]
    public void Generate_with_smartenum_should_generate()
    {
        var autoFaker = new AutoFaker();

        SetOverride(autoFaker);

        var order = autoFaker.Generate<CustomOrder>();
        order.DaysOfWeek.Should().NotBeNull();
        order.NullableDaysOfWeek.Should().NotBeNull();
        order.Longitude.Should().NotBeNull();
        CustomOrder.Constant.Should().Be("Order2x89ei");
            }

    private void SetOverride(AutoFaker autoFaker)
    {
        autoFaker.Configure(c => c.WithOverride(new BaseCustomOrderOverride()));
        autoFaker.Configure(c => c.WithOverride(new LongitudeOverride()));
        autoFaker.Configure(c => c.WithOverride(new CustomOrderOverride()));
    }
}