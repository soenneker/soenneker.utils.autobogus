using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;
using Binder = Bogus.Binder;

namespace Soenneker.Utils.AutoBogus;

/// <summary>
/// A class for binding generated instances.
/// </summary>
public class AutoBinder : Binder, IAutoBinder
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to create.</typeparam>
    /// <param name="context">The <see cref="AutoGenerateContext"/> instance for the generate request.</param>
    /// <returns>The created instance of <typeparamref name="TType"/>.</returns>
    public virtual TType CreateInstance<TType>(AutoGenerateContext? context)
    {
        if (context != null)
        {
            Type type = typeof(TType);
            ConstructorInfo constructor = GetConstructor<TType>();

            if (constructor != null)
            {
                ParameterInfo[] parametersInfo = constructor.GetParameters();
                object[] parameters = new object[parametersInfo.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = GetParameterGenerator(type, parametersInfo[i], context).Generate(context);
                }

                return (TType)constructor.Invoke(parameters);
            }
        }

        return default;
    }

    /// <summary>
    /// Populates the provided instance with generated values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to populate.</typeparam>
    /// <param name="instance">The instance to populate.</param>
    /// <param name="context">The <see cref="AutoGenerateContext"/> instance for the generate request.</param>
    /// <param name="members">An optional collection of members to populate. If null, all writable instance members are populated.</param>
    /// <remarks>
    /// Due to the boxing nature of value types, the <paramref name="instance"/> parameter is an object. This means the populated
    /// values are applied to the provided instance and not a copy.
    /// </remarks>
    public virtual void PopulateInstance<TType>(object instance, AutoGenerateContext context, MemberInfo[]? members = null)
    {
        Type type = typeof(TType);

        // We can only populate non-null instances 
        if (instance == null || context == null)
        {
            return;
        }

        // Iterate the members and bind a generated value
        List<AutoMember> autoMembers = GetMembersToPopulate(type, members);

        foreach (AutoMember? member in autoMembers)
        {
            if (member.Type != null)
            {
                // Check if the member has a skip config or the type has already been generated as a parent
                // If so skip this generation otherwise track it for use later in the object tree
                if (ShouldSkip(member.Type, $"{type.FullName}.{member.Name}", context))
                {
                    continue;
                }

                context.ParentType = type;
                context.GenerateType = member.Type;
                context.GenerateName = member.Name;

                context.TypesStack.Push(member.Type);

                // Generate a random value and bind it to the instance
                IAutoGenerator generator = AutoGeneratorFactory.GetGenerator(context);
                object value = generator.Generate(context);

                try
                {
                    if (!member.IsReadOnly)
                    {
                        member.Setter.Invoke(instance, value);
                    }
                    else if (member.Type.IsDictionary())
                    {
                        PopulateDictionary(value, instance, member);
                    }
                    else if (member.Type.IsCollection())
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
    }

    private static bool ShouldSkip(Type type, string path, AutoGenerateContext context)
    {
        // Skip if the type is found
        if (context.Config.SkipTypes.Contains(type))
        {
            return true;
        }

        // Skip if the path is found
        if (context.Config.SkipPaths.Contains(path))
        {
            return true;
        }

        //check if tree depth is reached
        int? treeDepth = context.Config.TreeDepth.Invoke(context);

        if (treeDepth.HasValue && context.TypesStack.Count() >= treeDepth)
            return true;

        // Finally check if the recursive depth has been reached

        int count = context.TypesStack.Count(t => t == type);
        int recursiveDepth = context.Config.RecursiveDepth.Invoke(context);

        return count >= recursiveDepth;
    }

    //private ConstructorInfo GetConstructor<TType>()
    //{
    //    Type type = typeof(TType);
    //    ConstructorInfo[] constructors = type.GetConstructors();

    //    // For dictionaries and enumerables locate a constructor that is used for populating as well
    //    if (type.IsDictionary())
    //    {
    //        return ResolveTypedConstructor(typeof(IDictionary<,>), constructors);
    //    }

    //    if (type.IsEnumerable())
    //    {
    //        return ResolveTypedConstructor(typeof(IEnumerable<>), constructors);
    //    }

    //    // Attempt to find a default constructor
    //    // If one is not found, simply use the first in the list
    //    ConstructorInfo? defaultConstructor = (from c in constructors
    //        let p = c.GetParameters()
    //        where p.Count() == 0
    //        select c).SingleOrDefault();

    //    return defaultConstructor ?? constructors.FirstOrDefault();
    //}

    private static ConstructorInfo GetConstructor<TType>()
    {
        Type type = typeof(TType);
        ConstructorInfo[] constructors = type.GetConstructors();

        if (type.IsDictionary())
        {
            return ResolveTypedConstructor(typeof(IDictionary<,>), constructors);
        }

        if (type.IsEnumerable())
        {
            return ResolveTypedConstructor(typeof(IEnumerable<>), constructors);
        }

        foreach (ConstructorInfo constructor in constructors)
        {
            if (constructor.GetParameters().Length == 0)
            {
                return constructor;
            }
        }

        return constructors.Length > 0 ? constructors[0] : null;
    }

    //private static ConstructorInfo ResolveTypedConstructor(Type type, IEnumerable<ConstructorInfo> constructors)
    //{
    //    // Find the first constructor that matches the passed generic definition
    //    return (from c in constructors
    //        let p = c.GetParameters()
    //        where p.Count() == 1
    //        let m = p.Single()
    //        where m.ParameterType.IsGenericType()
    //        let d = m.ParameterType.GetGenericTypeDefinition()
    //        where d == type
    //        select c).SingleOrDefault();
    //}

    private static ConstructorInfo ResolveTypedConstructor(Type type, ConstructorInfo[] constructors)
    {
        for (int i = 0; i < constructors.Length; i++)
        {
            ConstructorInfo c = constructors[i];
            ParameterInfo[] parameters = c.GetParameters();

            if (parameters.Length == 1)
            {
                ParameterInfo parameter = parameters[0];
                Type parameterType = parameter.ParameterType;

                if (parameterType.IsGenericType)
                {
                    Type genericTypeDefinition = parameterType.GetGenericTypeDefinition();

                    if (genericTypeDefinition == type)
                    {
                        return c;
                    }
                }
            }
        }

        return null;
    }

    private static IAutoGenerator GetParameterGenerator(Type type, ParameterInfo parameter, AutoGenerateContext context)
    {
        context.ParentType = type;
        context.GenerateType = parameter.ParameterType;
        context.GenerateName = parameter.Name;

        return AutoGeneratorFactory.GetGenerator(context);
    }

    private List<AutoMember> GetMembersToPopulate(Type type, MemberInfo[]? members)
    {
        // If a list of members is provided, no others should be populated
        if (members != null)
        {
            var autoMembersList = new List<AutoMember>(members.Length);
            for (int i = 0; i < members.Length; i++)
            {
                autoMembersList.Add(new AutoMember(members[i]));
            }
            return autoMembersList;
        }

        // Get the baseline members resolved by Bogus
        var autoMembers = new List<AutoMember>();

        foreach (MemberInfo? member in GetMembers(type).Values)
        {
            autoMembers.Add(new AutoMember(member));
        }

        MemberInfo[] memberInfos = type.GetMembers(BindingFlags);
        int length = memberInfos.Length;

        for (int i = 0; i < length; i++)
        {
            MemberInfo member = memberInfos[i];

            // Then check if any other members can be populated
            var autoMember = new AutoMember(member);

            bool found = false;
            for (int j = 0; j < autoMembers.Count; j++)
            {
                if (autoMembers[j].Name == autoMember.Name)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // A readonly dictionary or collection member can use the Add() method
                if (autoMember.IsReadOnly && autoMember.Type.IsDictionary())
                {
                    autoMembers.Add(autoMember);
                }
                else if (autoMember.IsReadOnly && autoMember.Type.IsCollection())
                {
                    autoMembers.Add(autoMember);
                }
            }
        }

        return autoMembers;
    }

    private static void PopulateDictionary(object value, object parent, AutoMember member)
    {
        object? instance = member.Getter(parent);
        Type[] argTypes = GetAddMethodArgumentTypes(member.Type);
        MethodInfo? addMethod = GetAddMethod(member.Type, argTypes);

        if (instance != null && addMethod != null && value is IDictionary dictionary)
        {
            foreach (object? key in dictionary.Keys)
            {
                addMethod.Invoke(instance, [key, dictionary[key]]);
            }
        }
    }

    private static void PopulateCollection(object value, object parent, AutoMember member)
    {
        object? instance = member.Getter(parent);
        Type[] argTypes = GetAddMethodArgumentTypes(member.Type);
        MethodInfo? addMethod = GetAddMethod(member.Type, argTypes);

        if (instance != null && addMethod != null && value is ICollection collection)
        {
            foreach (object? item in collection)
            {
                addMethod.Invoke(instance, [item]);
            }
        }
    }

    private static MethodInfo? GetAddMethod(Type type, Type[] argTypes)
    {
        MethodInfo? method = type.GetMethod("Add", argTypes);

        if (method != null)
            return method;

        Type[] interfaces = type.GetInterfaces();

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

    private static Type[] GetAddMethodArgumentTypes(Type type)
    {
        if (!type.IsGenericType())
            return [typeof(object)];

        return type.GetGenericArguments();
    }
}