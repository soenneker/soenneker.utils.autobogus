using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Soenneker.Utils.AutoBogus.Util;

internal static class ReflectionHelper
{
    internal static bool IsEnum(Type type)
    {
        return type.IsEnum;
    }

    internal static bool IsAbstract(Type type)
    {
        return type.IsAbstract;
    }

    internal static bool IsInterface(Type type)
    {
        return type.IsInterface;
    }

    internal static bool IsGenericType(Type type)
    {
        return type.IsGenericType;
    }

    internal static bool IsExpandoObject(Type type)
    {
        return type == typeof(ExpandoObject);
    }

    internal static bool IsAssignableFrom(Type baseType, Type type)
    {
        return baseType.IsAssignableFrom(type);
    }

    internal static bool IsField(MemberInfo member)
    {
        return member.MemberType == MemberTypes.Field;
    }

    internal static bool IsProperty(MemberInfo member)
    {
        return member.MemberType == MemberTypes.Property;
    }

    internal static IEnumerable<Type> GetGenericArguments(Type type)
    {
        return type.GetGenericArguments();
    }

    internal static Type GetGenericTypeDefinition(Type type)
    {
        return type.GetGenericTypeDefinition();
    }

    internal static Type GetGenericCollectionType(Type type)
    {
        IEnumerable<Type> interfaces = type.GetInterfaces().Where(ReflectionHelper.IsGenericType);

        if (IsInterface(type))
            interfaces = interfaces.Concat(new[] {type});

        Type? dictionaryType = null;
        Type? readOnlyDictionaryType = null;
        Type? listType = null;
        Type? setType = null;
        Type? collectionType = null;
        Type? enumerableType = null;

        foreach (Type? interfaceType in interfaces.Where(IsGenericType))
        {
            if (IsDictionary(interfaceType))
                dictionaryType = interfaceType;

            if (IsReadOnlyDictionary(interfaceType))
                readOnlyDictionaryType = interfaceType;

            if (IsList(interfaceType))
                listType = interfaceType;

            if (IsSet(interfaceType))
                setType = interfaceType;

            if (IsCollection(interfaceType))
                collectionType = interfaceType;

            if (IsEnumerable(interfaceType))
                enumerableType = interfaceType;
        }

        if ((dictionaryType != null) && (readOnlyDictionaryType != null) && IsReadOnlyDictionary(type))
            dictionaryType = null;

        return dictionaryType ?? readOnlyDictionaryType ?? listType ?? setType ?? collectionType ?? enumerableType;
    }

    internal static bool IsDictionary(Type type)
    {
        Type baseType = typeof(IDictionary<,>);
        return IsGenericTypeDefinition(baseType, type);
    }

    internal static bool IsReadOnlyDictionary(Type type)
    {
        Type baseType = typeof(IReadOnlyDictionary<,>);

        if (IsGenericTypeDefinition(baseType, type))
        {
            // Read only dictionaries don't have an Add() method
            IEnumerable<MethodInfo> methods = type
                .GetMethods()
                .Where(m => m.Name.Equals("Add"));

            return !methods.Any();
        }

        return false;
    }

    internal static bool IsSet(Type type)
    {
        Type baseType = typeof(ISet<>);
        return IsGenericTypeDefinition(baseType, type);
    }

    internal static bool IsList(Type type)
    {
        Type baseType = typeof(IList<>);
        return IsGenericTypeDefinition(baseType, type);
    }

    internal static bool IsCollection(Type type)
    {
        Type baseType = typeof(ICollection<>);
        return IsGenericTypeDefinition(baseType, type);
    }

    internal static bool IsEnumerable(Type type)
    {
        Type baseType = typeof(IEnumerable<>);
        return IsGenericTypeDefinition(baseType, type);
    }

    internal static bool IsNullable(Type type)
    {
        return IsGenericTypeDefinition(typeof(Nullable<>), type);
    }

    private static bool IsGenericTypeDefinition(Type baseType, Type type)
    {
        if (IsGenericType(type))
        {
            Type definition = GetGenericTypeDefinition(type);

            // Do an assignable query first
            if (IsAssignableFrom(baseType, definition))
            {
                return true;
            }

            // If that don't work use the more complex interface checks
            IEnumerable<Type> interfaces = (from i in type.GetInterfaces()
                where IsGenericTypeDefinition(baseType, i)
                select i);

            return interfaces.Any();
        }

        return false;
    }
}