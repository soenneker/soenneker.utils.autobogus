using System;
using System.Reflection;
using Soenneker.Reflection.Cache.Fields;
using Soenneker.Reflection.Cache.Properties;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus;

internal sealed class AutoMember
{
    internal readonly string Name;

    internal readonly CachedType CachedType;

    internal readonly CachedType ParentType;

    internal readonly bool IsReadOnly;

    internal readonly Func<object, object?> Getter;

    internal readonly Action<object, object?>? Setter;

    internal bool ShouldSkip;

    internal readonly bool IsDictionary;

    internal readonly bool IsCollection;

    internal AutoMember(CachedField cachedField, CachedType parentType, CacheService cacheService, AutoFakerConfig config)
    {
        Name = cachedField.FieldInfo.Name;

        CachedType = cacheService.Cache.GetCachedType(cachedField.FieldInfo.FieldType);
        ParentType = parentType;
        IsReadOnly = !cachedField.FieldInfo.IsPrivate && cachedField.FieldInfo.IsInitOnly;
        Getter = cachedField.FieldInfo.GetValue;

        if (!IsReadOnly)
            Setter = cachedField.FieldInfo.SetValue;

        IsDictionary = CachedType.IsDictionary;
        IsCollection = CachedType.IsCollection;

        SetShouldSkip(config);
    }

    internal AutoMember(CachedProperty cachedProperty, CachedType parentType, CacheService cacheService, AutoFakerConfig config)
    {
        PropertyInfo pi = cachedProperty.PropertyInfo;
        PropertyInfo effective = ResolvePropertyForAccessors(pi);

        Name = pi.Name;

        CachedType = cacheService.Cache.GetCachedType(pi.PropertyType);
        ParentType = parentType;
        IsReadOnly = effective.GetSetMethod(nonPublic: true) is null;
        Getter = obj => effective.GetValue(obj, []);

        if (!IsReadOnly)
            Setter = (obj, value) => effective.SetValue(obj, value, []);

        IsDictionary = CachedType.IsDictionary;
        IsCollection = CachedType.IsCollection;

        SetShouldSkip(config);
    }

    private static PropertyInfo ResolvePropertyForAccessors(PropertyInfo pi)
    {
        if (pi.GetSetMethod(nonPublic: true) != null)
            return pi;

        Type? declaring = pi.DeclaringType;
        if (declaring == null)
            return pi;

        PropertyInfo? declared = declaring.GetProperty(pi.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        return declared?.GetSetMethod(nonPublic: true) != null ? declared : pi;
    }

    private void SetShouldSkip(AutoFakerConfig config)
    {
        if (config.SkipTypes != null && config.SkipTypes.Contains(CachedType.Type))
        {
            ShouldSkip = true;
        }

        // Skip if the path is found
        if (config.SkipPaths != null && config.SkipPaths.Contains($"{ParentType.Type.FullName}.{Name}"))
        {
            ShouldSkip = true;
        }
    }
}