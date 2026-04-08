using Soenneker.Utils.AutoBogus.Attributes;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

[AutoFakerIncludeInheritedProperties(false)]
public sealed class TestConcreteRestrictedSetterDerivedForceExclude : TestAbstractRestrictedSetterBase
{
    public string Derived { get; set; }
}
