using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Utils;

internal static class GenericTypeUtil
{
    internal static (CachedType?, GenericCollectionType?) GetGenericCollectionType(CachedType cachedType)
    {
        CachedType[] interfaces = cachedType.GetCachedInterfaces()!;

        var interfacesList = new List<CachedType>(interfaces.Length + 1);

        if (cachedType.IsGenericType)
            interfacesList.Add(cachedType);

        for (int i = 0; i < interfaces.Length; i++)
        {
            CachedType interfaceType = interfaces[i];

            interfacesList.Add(interfaceType);
        }

        if (interfaces.Length == 0)
            return (null, null);

        (CachedType?, GenericCollectionType?) result = interfacesList.GetTypeOfGenericCollectionFromInterfaceTypes();

        return result;
    }
}