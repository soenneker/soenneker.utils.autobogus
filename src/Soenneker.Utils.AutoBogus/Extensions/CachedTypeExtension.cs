using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class CachedTypeExtension
{
    public static CachedType[] GetAddMethodArgumentTypes(this CachedType type)
    {
        if (!type.IsGenericType)
            return [CachedTypeService.Object.Value];

        return type.GetCachedGenericArguments()!;
    }
}