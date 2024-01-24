using FluentAssertions;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
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
    public void Generate_Product_should_generate()
    {
        IAutoFaker faker = AutoFaker.Create();

        var product = faker.Generate<Product>();
        product.Should().NotBeNull();
        product.Reviews.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Generate_many_should_generate()
    {
        IAutoFaker? faker = AutoFaker.Create();

        for (var i = 0; i < 100; i++)
        {
            var order = faker.Generate<Order>();
        }
    }
}