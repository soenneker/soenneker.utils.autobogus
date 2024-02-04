using System;
using System.Reflection;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus;

internal sealed class AutoMember
{
    internal readonly string Name;

    //internal Type Type { get; }

    internal readonly CachedType CachedType;

    internal readonly bool IsReadOnly;

    internal readonly Func<object, object> Getter;

    internal readonly Action<object, object> Setter;

    //internal AutoMember(CachedMember cachedMember)
    //{
    //    Name = cachedMember.Name;

    //    // Extract the required member info
    //    if (cachedMember.IsField)
    //    {
    //        var fieldInfo = cachedMember.MemberInfo as FieldInfo;

    //        Type = fieldInfo.FieldType;
    //        CachedType = CacheService.Cache.GetCachedType(Type);
    //        IsReadOnly = !fieldInfo.IsPrivate && fieldInfo.IsInitOnly;
    //        Getter = fieldInfo.GetValue;
    //        Setter = fieldInfo.SetValue;
    //    }
    //    else if (cachedMember.IsProperty)
    //    {
    //        var propertyInfo = cachedMember.MemberInfo as PropertyInfo;

    //        Type = propertyInfo.PropertyType;
    //        CachedType = CacheService.Cache.GetCachedType(Type);
    //        IsReadOnly = !propertyInfo.CanWrite;
    //        Getter = obj => propertyInfo.GetValue(obj, new object[0]);
    //        Setter = (obj, value) => propertyInfo.SetValue(obj, value, new object[0]);
    //    }
    //}

    internal AutoMember(FieldInfo fieldInfo)
    {
        Name = fieldInfo.Name;

        // Extract the required member info


        CachedType = CacheService.Cache.GetCachedType(fieldInfo.FieldType);
        IsReadOnly = !fieldInfo.IsPrivate && fieldInfo.IsInitOnly;
        Getter = fieldInfo.GetValue;

        if (!IsReadOnly)
            Setter = fieldInfo.SetValue;
    }

    internal AutoMember(PropertyInfo propertyInfo)
    {
        Name = propertyInfo.Name;

        CachedType = CacheService.Cache.GetCachedType(propertyInfo.PropertyType);
        IsReadOnly = !propertyInfo.CanWrite;
        Getter = obj => propertyInfo.GetValue(obj, []);

        if (!IsReadOnly)
            Setter = (obj, value) => propertyInfo.SetValue(obj, value, []);
    }
}