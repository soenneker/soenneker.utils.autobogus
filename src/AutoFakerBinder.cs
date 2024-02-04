using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Soenneker.Reflection.Cache.Constructors;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus;

/// <summary>
/// A class for binding generated instances.
/// </summary>
public class AutoFakerBinder : IAutoFakerBinder
{
    private readonly AutoFakerConfig _autoFakerConfig;

    public AutoFakerBinder(AutoFakerConfig autoFakerConfig)
    {
        _autoFakerConfig = autoFakerConfig;
    }

    /// <summary>
    /// Creates an instance of <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to create.</typeparam>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <returns>The created instance of <typeparamref name="TType"/>.</returns>
    public TType? CreateInstance<TType>(AutoFakerContext? context)
    {
        if (context == null)
            return default;

        Type type = typeof(TType);

        CachedConstructor? constructor = GetConstructor(context.CachedType);

        if (constructor == null)
            return default;

        ParameterInfo[] parametersInfo = constructor.GetParameters();
        var parameters = new object[parametersInfo.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            parameters[i] = GetParameterGenerator(type, parametersInfo[i], context).Generate(context);
        }

        return (TType?) constructor.Invoke(parameters);
    }

    /// <summary>
    /// Populates the provided instance with generated values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to populate.</typeparam>
    /// <param name="instance">The instance to populate.</param>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <param name="members">An optional collection of members to populate. If null, all writable instance members are populated.</param>
    /// <remarks>
    /// Due to the boxing nature of value types, the <paramref name="instance"/> parameter is an object. This means the populated
    /// values are applied to the provided instance and not a copy.
    /// </remarks>
    public void PopulateInstance<TType>(object? instance, AutoFakerContext? context)
    {
        // We can only populate non-null instances 
        if (instance == null || context == null)
        {
            return;
        }

        Type type = typeof(TType);

        CachedType cachedType = CacheService.Cache.GetCachedType(type);

        // Iterate the members and bind a generated value
        List<AutoMember> autoMembers = GetMembersToPopulate(cachedType);

        foreach (AutoMember? member in autoMembers)
        {
            // Check if the member has a skip config or the type has already been generated as a parent
            // If so skip this generation otherwise track it for use later in the object tree
            if (ShouldSkip(member.CachedType, $"{type.FullName}.{member.Name}", context))
            {
                continue;
            }

            context.Setup(type, member.CachedType, member.Name);

            context.TypesStack.Push(member.CachedType.CacheKey.Value);

            // Generate a random value and bind it to the instance
            IAutoFakerGenerator generator = AutoFakerGeneratorFactory.GetGenerator(context);
            object value = generator.Generate(context);

            try
            {
                if (!member.IsReadOnly)
                {
                    member.Setter.Invoke(instance, value);
                }
                else if (member.CachedType.IsDictionary)
                {
                    PopulateDictionary(value, instance, member);
                }
                else if (member.CachedType.IsCollection())
                {
                    PopulateCollection(value, instance, member);
                }
            }
            catch
            {
            }

            // Remove the current type from the type stack so siblings can be created
            context.TypesStack.Pop();
        }
    }

    private bool ShouldSkip(CachedType cachedType, string path, AutoFakerContext context)
    {
        // Skip if the type is found
        if (_autoFakerConfig.SkipTypes != null && _autoFakerConfig.SkipTypes.Contains(cachedType.Type))
        {
            return true;
        }

        // Skip if the path is found
        if (_autoFakerConfig.SkipPaths != null && _autoFakerConfig.SkipPaths.Contains(path))
        {
            return true;
        }

        //check if tree depth is reached
        int? treeDepth = _autoFakerConfig.TreeDepth;

        if (treeDepth != null)
        {
            if (context.TypesStack.Count >= treeDepth)
                return true;
        }

        if (context.TypesStack.Count == 0)
            return false;

        // Finally check if the recursive depth has been reached

        int count = context.TypesStack.Count(c => c == cachedType.CacheKey);
        int recursiveDepth = _autoFakerConfig.RecursiveDepth;

        if (count >= recursiveDepth)
            return true;

        return false;
    }

    private static CachedConstructor? GetConstructor(CachedType type)
    {
        CachedConstructor[]? constructors = type.GetCachedConstructors();

        if (type.IsDictionary)
        {
            return ResolveTypedConstructor(typeof(IDictionary<,>), constructors);
        }

        if (type.IsEnumerable)
        {
            return ResolveTypedConstructor(typeof(IEnumerable<>), constructors);
        }

        foreach (CachedConstructor constructor in constructors)
        {
            if (constructor.GetParameters().Length == 0)
            {
                return constructor;
            }
        }

        return constructors.Length > 0 ? constructors[0] : null;
    }

    private static CachedConstructor? ResolveTypedConstructor(Type type, CachedConstructor[] constructors)
    {
        for (int i = 0; i < constructors.Length; i++)
        {
            CachedConstructor c = constructors[i];

            ParameterInfo[] parameters = c.GetParameters();

            if (parameters.Length != 1)
                continue;

            ParameterInfo parameter = parameters[0];
            Type parameterType = parameter.ParameterType;

            if (!parameterType.IsGenericType)
                continue;

            Type genericTypeDefinition = parameterType.GetGenericTypeDefinition();

            if (genericTypeDefinition == type)
            {
                return c;
            }
        }

        return null;
    }

    private static IAutoFakerGenerator GetParameterGenerator(Type type, ParameterInfo parameter, AutoFakerContext context)
    {
        context.Setup(type, CacheService.Cache.GetCachedType(parameter.ParameterType), parameter.Name);

        return AutoFakerGeneratorFactory.GetGenerator(context);
    }

    private static List<AutoMember> GetMembersToPopulate(CachedType cachedType)
    {
        // If a list of members is provided, no others should be populated
        //if (members != null)
        //{
        //    var autoMembersList = new List<AutoMember>(members.Length);

        //    for (int i = 0; i < members.Length; i++)
        //    {
        //        autoMembersList.Add(new AutoMember(members[i]));
        //    }

        //    return autoMembersList;
        //}

        // Get the baseline members resolved by Bogus
        var autoMembers = new List<AutoMember>();

        var properties = cachedType.GetProperties()!;

        var fields = cachedType.GetFields();

        foreach (var property in properties)
        {
            autoMembers.Add(new AutoMember(property));
        }

        foreach (var field in fields)
        {
            autoMembers.Add(new AutoMember(field));
        }

        //int length = memberInfos.Length;

        //for (int i = 0; i < length; i++)
        //{
        //    MemberInfo member = memberInfos[i];

        //    // Then check if any other members can be populated
        //    var autoMember = new AutoMember(member);

        //    bool found = false;
        //    for (int j = 0; j < autoMembers.Count; j++)
        //    {
        //        if (autoMembers[j].Name == autoMember.Name)
        //        {
        //            found = true;
        //            break;
        //        }
        //    }

        //    if (!found)
        //    {
        //        // A readonly dictionary or collection member can use the Add() method
        //        if (autoMember.IsReadOnly && autoMember.CachedType.IsDictionary)
        //        {
        //            autoMembers.Add(autoMember);
        //        }
        //        else if (autoMember.IsReadOnly && autoMember.CachedType.IsCollection())
        //        {
        //            autoMembers.Add(autoMember);
        //        }
        //    }
        //}

        return autoMembers;
    }

    private static void PopulateDictionary(object value, object parent, AutoMember member)
    {
        if (value is not IDictionary dictionary)
            return;

        object? instance = member.Getter(parent);
        Type[] argTypes = member.CachedType.GetAddMethodArgumentTypes();
        MethodInfo? addMethod = GetAddMethod(member.CachedType.Type, argTypes);

        if (instance != null && addMethod != null)
        {
            foreach (object? key in dictionary.Keys)
            {
                addMethod.Invoke(instance, [key, dictionary[key]]);
            }
        }
    }

    private static void PopulateCollection(object value, object parent, AutoMember member)
    {
        if (value is not ICollection collection)
            return;

        object? instance = member.Getter(parent);
        Type[] argTypes = member.CachedType.GetAddMethodArgumentTypes();
        MethodInfo? addMethod = GetAddMethod(member.CachedType.Type, argTypes);

        if (instance != null && addMethod != null)
        {
            foreach (object? item in collection)
            {
                addMethod.Invoke(instance, [item]);
            }
        }
    }

    private static MethodInfo? GetAddMethod(Type cachedType, Type[] argTypes)
    {
        MethodInfo? method = cachedType.GetMethod("Add", argTypes);

        if (method != null)
            return method;

        var interfaces = cachedType.GetInterfaces()!;

        for (int i = 0; i < interfaces.Length; i++)
        {
            MethodInfo? interfaceMethod = GetAddMethod(interfaces[i], argTypes);

            if (interfaceMethod != null)
            {
                return interfaceMethod;
            }
        }

        return method;
    }
}