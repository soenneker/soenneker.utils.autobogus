using Soenneker.Utils.AutoBogus.Attributes;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

[AutoFakerIncludeInheritedProperties(true)]
public sealed class TestConcreteRestrictedSetterDerivedForceInclude : TestAbstractRestrictedSetterBase
{
    public string Derived { get; set; }
}
