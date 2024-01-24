using Soenneker.Utils.AutoBogus.Enums;
using Soenneker.Utils.AutoBogus.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Soenneker.Utils.AutoBogus.Util;

internal static class ReflectionHelper
{
    internal static bool IsCollection(this Type type)
    {
        Type baseType = typeof(ICollection<>);
        return IsGenericTypeDefinition(baseType, type);
    }

    internal static bool IsEnumerable(this Type type)
    {
        Type baseType = typeof(IEnumerable<>);
        return IsGenericTypeDefinition(baseType, type);
    }

    internal static bool IsNullable(this Type type)
    {
        return IsGenericTypeDefinition(typeof(Nullable<>), type);
    }

    internal static bool IsAssignableFrom(Type baseType, Type type)
    {
        // return AutoFaker.Cache.GetCachedType(baseType).IsAssignableFrom(type);

        return baseType.IsAssignableFrom(type);
    }

    internal static bool IsField(this MemberInfo member)
    {
        return member.MemberType == MemberTypes.Field;
    }

    internal static bool IsProperty(this MemberInfo member)
    {
        return member.MemberType == MemberTypes.Property;
    }

    internal static Type[] GetGenericArguments(this Type type)
    {
        //  return AutoFaker.Cache.GetCachedType(type).GetGenericArguments();

        return type.GetGenericArguments();
    }

    internal static Type GetGenericTypeDefinition(this Type type)
    {
        //return AutoFaker.Cache.GetCachedType(type).GetGenericTypeDefinition();

        return type.GetGenericTypeDefinition();
    }

    //internal static (Type?, GenericCollectionType?) GetGenericCollectionType(Type type)
    //{
    //    List<Type> cachedInterfaces = type.GetInterfaces().Where(c => c.IsGenericType).ToList();

    //    if (type.IsGenericType)
    //        cachedInterfaces.Add(type);

    //    return cachedInterfaces.GetTypeOfGenericCollectionFromInterfaceTypes();
    //}

    internal static (Type?, GenericCollectionType?) GetGenericCollectionType(Type type)
    {
        Type[] interfaces = type.GetInterfaces();

        List<Type> interfacesList = new List<Type>(interfaces.Length + 1);

        if (type.IsGenericType)
            interfacesList.Add(type);

        for (int i = 0; i < interfaces.Length; i++)
        {
            Type interfaceType = interfaces[i];

            if (interfaceType.IsGenericType)
                interfacesList.Add(interfaceType);
        }

        (Type?, GenericCollectionType?) result = interfacesList.GetTypeOfGenericCollectionFromInterfaceTypes();

        return result;
    }


    // Original
    //internal static Type GetGenericCollectionType(Type type)
    //{
    //    //Type[]? cachedInterfaces = AutoFaker.Cache.GetCachedType(type).GetInterfaces();

    //    var cachedInterfaces = type.GetInterfaces();

    //    IEnumerable<Type> interfaces = cachedInterfaces.Where(ReflectionHelper.IsGenericType);

    //    if (IsInterface(type))
    //        interfaces = interfaces.Concat(new[] { type });

    //    Type? dictionaryType = null;
    //    Type? readOnlyDictionaryType = null;
    //    Type? listType = null;
    //    Type? setType = null;
    //    Type? collectionType = null;
    //    Type? enumerableType = null;

    //    foreach (Type? interfaceType in interfaces.Where(IsGenericType))
    //    {
    //        if (IsDictionary(interfaceType))
    //            dictionaryType = interfaceType;

    //        if (IsReadOnlyDictionary(interfaceType))
    //            readOnlyDictionaryType = interfaceType;

    //        if (IsList(interfaceType))
    //            listType = interfaceType;

    //        if (IsSet(interfaceType))
    //            setType = interfaceType;

    //        if (IsCollection(interfaceType))
    //            collectionType = interfaceType;

    //        if (IsEnumerable(interfaceType))
    //            enumerableType = interfaceType;
    //    }

    //    if ((dictionaryType != null) && (readOnlyDictionaryType != null) && IsReadOnlyDictionary(type))
    //        dictionaryType = null;

    //    return dictionaryType ?? readOnlyDictionaryType ?? listType ?? setType ?? collectionType ?? enumerableType;
    //}

    //internal static bool IsDictionary(this Type type)
    //{
    //    Type baseType = typeof(IDictionary<,>);
    //    return IsGenericTypeDefinition(baseType, type);
    //}

    internal static bool IsReadOnlyDictionary(this Type type)
    {
        Type baseType = typeof(IReadOnlyDictionary<,>);

        if (IsGenericTypeDefinition(baseType, type))
        {
            //MethodInfo[] cachedMethods = AutoFaker.Cache.GetCachedType(type).GetMethods();

            MethodInfo[] cachedMethods = type.GetMethods();

            // Read-only dictionaries don't have an Add() method
            for (int i = 0; i < cachedMethods.Length; i++)
            {
                if (cachedMethods[i].Name.Equals("Add"))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    //private static bool IsGenericTypeDefinition(Type baseType, Type type)
    //{
    //    if (IsGenericType(type))
    //    {
    //        Type definition = GetGenericTypeDefinition(type);

    //        // Do an assignable query first
    //        if (IsAssignableFrom(baseType, definition))
    //        {
    //            return true;
    //        }

    //        //Type[]? cachedInterfaces = AutoFaker.Cache.GetCachedType(type).GetInterfaces();

    //        Type[] cachedInterfaces = type.GetInterfaces();

    //        // If that don't work use the more complex interface checks
    //        IEnumerable<Type> interfaces = from i in cachedInterfaces
    //                                       where IsGenericTypeDefinition(baseType, i)
    //                                       select i;

    //        return interfaces.Any();
    //    }

    //    return false;
    //}

    private static bool IsGenericTypeDefinition(Type baseType, Type type)
    {
        if (type.IsGenericType())
        {
            Type definition = GetGenericTypeDefinition(type);

            if (IsAssignableFrom(baseType, definition))
                return true;

            Type[] cachedInterfaces = type.GetInterfaces();

            for (int i = 0; i < cachedInterfaces.Length; i++)
            {
                if (IsGenericTypeDefinition(baseType, cachedInterfaces[i]))
                    return true;
            }

            return false;
        }

        return false;
    }
}