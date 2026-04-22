using AwesomeAssertions;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Tests.Utils;

public class GenericTypeUtilTests
{
    [Test]
    public void Generate_should_generate()
    {
        var faker = new AutoFaker();

        var order1 = faker.Generate<Order>();
        order1.Should().NotBeNull();
    }
}