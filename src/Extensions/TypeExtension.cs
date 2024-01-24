using System;
using System.Dynamic;

namespace Soenneker.Utils.AutoBogus.Extensions;

public static class TypeExtension
{
    public static bool IsOrDerivedFrom(this Type? sourceType, Type targetType)
    {
        return targetType.IsAssignableFrom(sourceType);
    }

    internal static bool IsEnum(this Type type)
    {
        return type.IsEnum;
    }

    internal static bool IsAbstract(this Type type)
    {
        return type.IsAbstract;
    }

    internal static bool IsInterface(this Type type)
    {
        return type.IsInterface;
    }

    internal static bool IsGenericType(this Type type)
    {
        return type.IsGenericType;
    }

    internal static bool IsExpandoObject(this Type type)
    {
        return type == typeof(ExpandoObject);
    }

    internal static bool IsDictionary(this Type type)
    {
        if (type.Name == "IDictionary`2")
            return true;

        var interfaces = type.GetInterfaces();

        foreach (var i in interfaces)
        {
            switch (i.Name)
            {
                case "IDictionary`2":
                    {
                        return true;
                    }
            }
        }

        return false;
    }
}