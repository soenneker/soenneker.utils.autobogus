using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple.Abstract;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

public record TestInterfaceRecord(ITestInterface Interface)
{
    public ITestInterface Interface { get; } = Interface;
}