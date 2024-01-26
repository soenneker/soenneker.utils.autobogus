using Soenneker.Utils.AutoBogus.Enums;
using System.Collections.Generic;
using Soenneker.Reflection.Cache.Types;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class TypeArrayExtension
{
    internal static (CachedType?, GenericCollectionType?) GetTypeOfGenericCollectionFromInterfaceTypes(this List<CachedType> types)
    {
        if (types.Count == 0)
            return (null, null);

        GenericCollectionType genericCollectionType = GenericCollectionType.Unknown;

        CachedType? returnType = null;

        foreach (CachedType type in types)
        {
            switch (type.Type.Name)
            {
                case "SortedList`2":
                    return (type, GenericCollectionType.SortedList);
                case "Dictionary`2":
                    return (type, GenericCollectionType.Dictionary);
                case "ReadOnlyDictionary`2":
                    return (type, GenericCollectionType.ReadOnlyDictionary);
                case "IDictionary`2":
                    if (genericCollectionType < GenericCollectionType.Dictionary)
                    {
                        genericCollectionType = GenericCollectionType.Dictionary;
                        returnType = type;
                    }
                    break;
                case "IImmutableDictionary`2":
                    if (genericCollectionType < GenericCollectionType.ImmutableDictionary)
                    {
                        genericCollectionType = GenericCollectionType.ImmutableDictionary;
                        returnType = type;
                    }
                    break;
                case "IReadOnlyDictionary`2":
                    if (genericCollectionType < GenericCollectionType.ReadOnlyDictionary)
                    {
                        genericCollectionType = GenericCollectionType.ReadOnlyDictionary;
                        returnType = type;
                    }
                    break;
                case "IReadOnlylist`1":
                    if (genericCollectionType < GenericCollectionType.ReadOnlyList)
                    {
                        genericCollectionType = GenericCollectionType.ReadOnlyList;
                        returnType = type;
                    }
                    break;
                case "Ilist`1":
                    if (genericCollectionType < GenericCollectionType.ListType)
                    {
                        genericCollectionType = GenericCollectionType.ListType;
                        returnType = type;
                    }
                    break;
                case "ISet`1":
                    if (genericCollectionType < GenericCollectionType.Set)
                    {
                        genericCollectionType = GenericCollectionType.Set;
                        returnType = type;
                    }
                    break;
                case "IReadOnlyCollection`1":
                    if (genericCollectionType < GenericCollectionType.ReadOnlyCollection)
                    {
                        genericCollectionType = GenericCollectionType.ReadOnlyCollection;
                        returnType = type;
                    }
                    break;
                case "ICollection`1":
                    if (genericCollectionType < GenericCollectionType.Collection)
                    {
                        genericCollectionType = GenericCollectionType.Collection;
                        returnType = type;
                    }
                    break;
                case "IEnumerable`1":
                    if (genericCollectionType < GenericCollectionType.Enumerable)
                    {
                        genericCollectionType = GenericCollectionType.Enumerable;
                        returnType = type;
                    }
                    break;
            }
        }

        return (returnType, genericCollectionType);
    }
}