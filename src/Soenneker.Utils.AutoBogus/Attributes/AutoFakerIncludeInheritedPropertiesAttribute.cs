using System;

namespace Soenneker.Utils.AutoBogus.Attributes;

/// <summary>
/// When applied to a type, overrides <see cref="Config.AutoFakerConfig.IncludeInheritedProperties"/> for generation of that type.
/// With <c>Inherited = true</c> on this attribute class, types derived from a decorated base also honor the override.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class AutoFakerIncludeInheritedPropertiesAttribute : Attribute
{
    /// <summary>
    /// Whether to merge instance properties from base types when populating this type.
    /// </summary>
    public bool Include { get; }

    /// <param name="include"><see langword="true"/> to include inherited properties; <see langword="false"/> to only populate members declared on the generated type.</param>
    public AutoFakerIncludeInheritedPropertiesAttribute(bool include)
    {
        Include = include;
    }
}
