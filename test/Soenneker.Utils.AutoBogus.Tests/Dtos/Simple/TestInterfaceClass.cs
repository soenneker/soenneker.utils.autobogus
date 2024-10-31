using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Abstract;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public class TestInterfaceClass(ITestInterface i)
{
    public ITestInterface Interface { get; } = i;
}