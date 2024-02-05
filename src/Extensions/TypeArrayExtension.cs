using Soenneker.Utils.AutoBogus.Enums;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class TypeArrayExtension
{
    internal static (CachedType?, GenericCollectionType?) GetTypeOfGenericCollectionFromInterfaceTypes(this List<CachedType> cachedTypes)
    {
        if (cachedTypes.Count == 0)
            return (null, null);

        GenericCollectionType genericCollectionType = GenericCollectionType.Unknown;

        CachedType? returnType = null;

        for (var i = 0; i < cachedTypes.Count; i++)
        {
            CachedType cachedType = cachedTypes[i];
            switch (cachedType.Type.Name)
            {
                case "SortedList`2":
                    return (cachedType, GenericCollectionType.SortedList);
                case "Dictionary`2":
                    return (cachedType, GenericCollectionType.Dictionary);
                case "ReadOnlyDictionary`2":
                    return (cachedType, GenericCollectionType.ReadOnlyDictionary);
                case "IDictionary`2":
                    if (genericCollectionType < GenericCollectionType.Dictionary)
                    {
                        genericCollectionType = GenericCollectionType.Dictionary;
                        returnType = cachedType;
                    }

                    break;
                case "IImmutableDictionary`2":
                    if (genericCollectionType < GenericCollectionType.ImmutableDictionary)
                    {
                        genericCollectionType = GenericCollectionType.ImmutableDictionary;
                        returnType = cachedType;
                    }

                    break;
                case "IReadOnlyDictionary`2":
                    if (genericCollectionType < GenericCollectionType.ReadOnlyDictionary)
                    {
                        genericCollectionType = GenericCollectionType.ReadOnlyDictionary;
                        returnType = cachedType;
                    }

                    break;
                case "IReadOnlylist`1":
                    if (genericCollectionType < GenericCollectionType.ReadOnlyList)
                    {
                        genericCollectionType = GenericCollectionType.ReadOnlyList;
                        returnType = cachedType;
                    }

                    break;
                case "Ilist`1":
                    if (genericCollectionType < GenericCollectionType.ListType)
                    {
                        genericCollectionType = GenericCollectionType.ListType;
                        returnType = cachedType;
                    }

                    break;
                case "ISet`1":
                    if (genericCollectionType < GenericCollectionType.Set)
                    {
                        genericCollectionType = GenericCollectionType.Set;
                        returnType = cachedType;
                    }

                    break;
                case "IReadOnlyCollection`1":
                    if (genericCollectionType < GenericCollectionType.ReadOnlyCollection)
                    {
                        genericCollectionType = GenericCollectionType.ReadOnlyCollection;
                        returnType = cachedType;
                    }

                    break;
                case "ICollection`1":
                    if (genericCollectionType < GenericCollectionType.Collection)
                    {
                        genericCollectionType = GenericCollectionType.Collection;
                        returnType = cachedType;
                    }

                    break;
                case "IEnumerable`1":
                    if (genericCollectionType < GenericCollectionType.Enumerable)
                    {
                        genericCollectionType = GenericCollectionType.Enumerable;
                        returnType = cachedType;
                    }

                    break;
            }
        }

        return (returnType, genericCollectionType);
    }
}