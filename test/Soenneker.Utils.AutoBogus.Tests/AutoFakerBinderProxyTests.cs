using Soenneker.Utils.AutoBogus.FakeItEasy;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Abstract;
using AwesomeAssertions;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerBinderProxyTests
{
    [Fact]
    public void RuleFor_should_set_property_on_interface()
    {
        var faker = new AutoFaker<ITestInterfaceWithProperty> { Binder = new FakeItEasyAutoFakerBinder() };

        faker.RuleFor(o => o.Name, () => "blah");

        ITestInterfaceWithProperty result = faker.Generate();

        result.Should()
              .NotBeNull();
        result.Name.Should()
              .Be("blah");
    }
}