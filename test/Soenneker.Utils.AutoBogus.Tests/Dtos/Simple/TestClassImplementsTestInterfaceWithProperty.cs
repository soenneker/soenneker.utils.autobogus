using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Abstract;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestClassImplementsTestInterfaceWithProperty : ITestInterfaceWithProperty
{
    public string Name { get; set; }
}