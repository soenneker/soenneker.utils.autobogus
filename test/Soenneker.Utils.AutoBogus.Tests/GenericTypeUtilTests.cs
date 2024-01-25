using FluentAssertions;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class GenericTypeUtilTests
{
    [Fact]
    public void Generate_should_generate()
    {
        IAutoFaker? faker = AutoFaker.Create();

        var order1 = faker.Generate<Order>();
        order1.Should().NotBeNull();

        //var order2 = faker.Generate<Order>();
    }
}