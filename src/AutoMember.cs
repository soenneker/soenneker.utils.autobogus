using System;
using System.Reflection;
using Soenneker.Reflection.Cache.Members;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus;

internal sealed class AutoMember
{
    internal string Name { get; }

    internal Type Type { get; }

    internal CachedType CachedType { get; }

    internal bool IsReadOnly { get; }

    internal Func<object, object> Getter { get; }

    internal Action<object, object> Setter { get; }

    internal AutoMember(CachedMember cachedMember)
    {
        Name = cachedMember.Name;

        // Extract the required member info
        if (cachedMember.IsField)
        {
            var fieldInfo = cachedMember.MemberInfo as FieldInfo;

            Type = fieldInfo.FieldType;
            CachedType = CacheService.Cache.GetCachedType(Type);
            IsReadOnly = !fieldInfo.IsPrivate && fieldInfo.IsInitOnly;
            Getter = fieldInfo.GetValue;
            Setter = fieldInfo.SetValue;
        }
        else if (cachedMember.IsProperty)
        {
            var propertyInfo = cachedMember.MemberInfo as PropertyInfo;

            Type = propertyInfo.PropertyType;
            CachedType = CacheService.Cache.GetCachedType(Type);
            IsReadOnly = !propertyInfo.CanWrite;
            Getter = obj => propertyInfo.GetValue(obj, new object[0]);
            Setter = (obj, value) => propertyInfo.SetValue(obj, value, new object[0]);
        }
    }

    //internal AutoMember(FieldInfo fieldInfo)
    //{
    //    Name = fieldInfo.Name;

    //    // Extract the required member info

    //    Type = fieldInfo.FieldType;
    //    CachedType = CacheService.Cache.GetCachedType(Type);
    //    IsReadOnly = !fieldInfo.IsPrivate && fieldInfo.IsInitOnly;
    //    Getter = fieldInfo.GetValue;
    //    Setter = fieldInfo.SetValue;
    //}

    //internal AutoMember(PropertyInfo propertyInfo)
    //{
    //    Name = propertyInfo.Name;

    //    Type = propertyInfo.PropertyType;
    //    CachedType = CacheService.Cache.GetCachedType(Type);
    //    IsReadOnly = !propertyInfo.CanWrite;
    //    Getter = obj => propertyInfo.GetValue(obj, []);
    //    Setter = (obj, value) => propertyInfo.SetValue(obj, value, []);
    //}
}