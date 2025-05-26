using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Utils;

public class GenericTypeUtilTests
{
    [Fact]
    public void Generate_should_generate()
    {
        var faker = new AutoFaker();

        var order1 = faker.Generate<Order>();
        order1.Should().NotBeNull();
    }
}