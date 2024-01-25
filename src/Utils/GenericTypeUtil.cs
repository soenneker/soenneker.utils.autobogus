using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus.Utils;

internal static class GenericTypeUtil
{
    internal static (Type?, GenericCollectionType?) GetGenericCollectionType(Type type)
    {
        Type[] interfaces = type.GetInterfaces();

        var interfacesList = new List<Type>(interfaces.Length + 1);

        if (type.IsGenericType)
            interfacesList.Add(type);

        for (int i = 0; i < interfaces.Length; i++)
        {
            Type interfaceType = interfaces[i];

            interfacesList.Add(interfaceType);
        }

        (Type?, GenericCollectionType?) result = interfacesList.GetTypeOfGenericCollectionFromInterfaceTypes();

        return result;
    }
}