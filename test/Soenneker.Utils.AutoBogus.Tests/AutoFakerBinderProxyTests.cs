using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Abstract;
using AwesomeAssertions;

namespace Soenneker.Utils.AutoBogus.Tests;

public class AutoFakerBinderProxyTests
{
    [Skip("Manual")]
    public void RuleFor_should_set_property_on_interface()
    {
        var faker = new AutoFaker<ITestInterfaceWithProperty>(){}; // Binder = new FakeItEasyBinder();

        faker.RuleFor(o => o.Name, () => "blah");

        ITestInterfaceWithProperty result = faker.Generate();

        result.Should()
              .NotBeNull();
        result.Name.Should()
              .Be("blah");
    }
}
