using FluentAssertions;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerTests
{
    [Fact]
    public void Generate_order_should_generate()
    {
        IAutoFaker faker = AutoFaker.Create();

        var order = faker.Generate<Order>();
        order.Should().NotBeNull();
    }


    [Fact]
    public void Generate_dictionary_should_generate()
    {
        IAutoFaker faker = AutoFaker.Create();

        var dictionary = faker.Generate<Dictionary<int, string>>();
        dictionary.Should().NotBeNull();
    }

    [Fact]
    public void Generate_Product_should_generate()
    {
        IAutoFaker faker = AutoFaker.Create();

        var product = faker.Generate<Product>();
        product.Should().NotBeNull();
        product.GetRevisions.Should().NotBeNullOrEmpty();
        // product.Reviews.Count.Should().BeGreaterThan(0);
       // product.
    }

    [Fact]
    public void Generate_many_Orders_should_generate()
    {
        IAutoFaker faker = AutoFaker.Create();

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
        IAutoFaker faker = AutoFaker.Create();

        List<int> intList = new List<int>();

        for (var i = 0; i < 1000; i++)
        {
            var generated = faker.Generate<int>();
            intList.Add(generated);
        }

        intList.Count.Should().Be(1000);
    }
}