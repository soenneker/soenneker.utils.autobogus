using System;
using System.Reflection;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus;

internal sealed class AutoMember
{
    internal string Name { get; }
    internal Type Type { get; }
    internal bool IsReadOnly { get; }
    internal Func<object, object> Getter { get; }
    internal Action<object, object> Setter { get; }

    internal AutoMember(MemberInfo memberInfo)
    {
        Name = memberInfo.Name;

        // Extract the required member info
        if (memberInfo.IsField())
        {
            var fieldInfo = memberInfo as FieldInfo;

            Type = fieldInfo.FieldType;
            IsReadOnly = !fieldInfo.IsPrivate && fieldInfo.IsInitOnly;
            Getter = fieldInfo.GetValue;
            Setter = fieldInfo.SetValue;
        }
        else if (memberInfo.IsProperty())
        {
            var propertyInfo = memberInfo as PropertyInfo;

            Type = propertyInfo.PropertyType;
            IsReadOnly = !propertyInfo.CanWrite;
            Getter = obj => propertyInfo.GetValue(obj, new object[0]);
            Setter = (obj, value) => propertyInfo.SetValue(obj, value, new object[0]);
        }
    }
}