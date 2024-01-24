using Soenneker.Utils.AutoBogus.Enums;
using System.Collections.Generic;
using System;

namespace Soenneker.Utils.AutoBogus.Extensions;

internal static class TypeArrayExtension
{
    internal static (Type?, GenericCollectionType?) GetTypeOfGenericCollectionFromInterfaceTypes(this List<Type> types)
    {
        if (types.Count == 0)
            return (null, null);

        GenericCollectionType genericCollectionType = GenericCollectionType.Unknown;

        Type? returnType = null;

        var index = 0;

        // TODO: sigh... this is ugly and it's a super hot path
        foreach (Type type in types)
        {
            switch (type.Name)
            {
                case "SortedList`2":
                    {
                        return (type, GenericCollectionType.SortedList);
                    }
                //case "Dictionary`2":
                //    {
                //        return (type, GenericCollectionType.Dictionary);
                //    }
                case "IImmutableDictionary`2":
                    {
                        return (type, GenericCollectionType.ImmutableDictionary);
                    }
                case "IReadOnlyDictionary`2":
                    {
                        return (type, GenericCollectionType.ReadOnlyDictionary);
                    }

                case "IDictionary`2":
                    {
                       // if (index == 0)
                     //       return (type, GenericCollectionType.Dictionary);

                        if (genericCollectionType < GenericCollectionType.Dictionary)
                        {
                            genericCollectionType = GenericCollectionType.Dictionary;
                            returnType = type;
                        }
                    }
                    break;
                case "IReadOnlylist`1":
                    {
                        if (genericCollectionType < GenericCollectionType.ReadOnlyList)
                        {
                            genericCollectionType = GenericCollectionType.ReadOnlyList;
                            returnType = type;
                        }

                        break;
                    }
                case "Ilist`1":
                    {
                        if (genericCollectionType < GenericCollectionType.ListType)
                        {
                            genericCollectionType = GenericCollectionType.ListType;
                            returnType = type;
                        }

                        break;
                    }
                case "ISet`1":
                    {
                        if (genericCollectionType < GenericCollectionType.Set)
                        {
                            genericCollectionType = GenericCollectionType.Set;
                            returnType = type;
                        }

                        break;
                    }
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

            index++;
        }

        return (returnType, genericCollectionType);
    }
}