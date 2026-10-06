using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class CachedTypeExtension
{
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Runtime reflection requires members that trimming may remove. Use statically registered metadata instead.")]
    public static CachedType[] GetAddMethodArgumentTypes(this CachedType type)
    {
        if (!type.IsGenericType)
            return [CachedTypeService.Object.Value];

        return type.GetCachedGenericArguments()!;
    }
}