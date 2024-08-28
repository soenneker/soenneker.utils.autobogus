using Soenneker.Utils.AutoBogus.Enums;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class TypeArrayExtension
{
    internal static (CachedType?, GenericCollectionType?) GetTypeOfGenericCollectionFromInterfaceTypes(this List<CachedType> cachedTypes)
    {
        var genericCollectionType = GenericCollectionType.Unknown;

        CachedType? returnType = null;

        for (var i = 0; i < cachedTypes.Count; i++)
        {
            CachedType cachedType = cachedTypes[i];

            switch (cachedType.Type.Name)
            {
                case "ImmutableArray`1":
                    return (cachedType, GenericCollectionType.ImmutableArray);
                case "IImmutableList`1":
                case "ImmutableList`1":
                    return (cachedType, GenericCollectionType.ImmutableList);
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
                case "ReadOnlyCollection`1":
                    return (cachedType, GenericCollectionType.ReadOnlyCollection);
                case "Collection`1":
                    return (cachedType, GenericCollectionType.Collection);
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
                case "IList`1":
                    if (genericCollectionType < GenericCollectionType.List)
                    {
                        genericCollectionType = GenericCollectionType.List;
                        returnType = cachedType;
                    }

                    break;
                case "ISet`1":
                case "IReadOnlySet`1":
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