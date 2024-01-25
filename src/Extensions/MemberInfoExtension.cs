using System.Reflection;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class MemberInfoExtension
{
    internal static bool IsField(this MemberInfo member)
    {
        return member.MemberType == MemberTypes.Field;
    }

    internal static bool IsProperty(this MemberInfo member)
    {
        return member.MemberType == MemberTypes.Property;
    }
}