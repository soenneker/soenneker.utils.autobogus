using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Soenneker.Reflection.Cache.Constructors;
using Soenneker.Reflection.Cache.Extensions;
using Soenneker.Reflection.Cache.Methods;
using Soenneker.Reflection.Cache.Parameters;
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

    private readonly CachedType _genericDictionary = CacheService.Cache.GetCachedType(typeof(IDictionary<,>));
    private readonly CachedType _enumerable = CacheService.Cache.GetCachedType(typeof(IEnumerable<>));

    private readonly Dictionary<CachedType, List<AutoMember>> _autoMembersCache = [];
    private readonly Dictionary<CachedType, CachedConstructor> _constructorsCache = [];

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

        CachedParameter[] cachedParameters = constructor.GetCachedParameters();

        if (cachedParameters.Length == 0)
        {
            return (TType?)constructor.Invoke();
        }

        var parameters = new object[cachedParameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            parameters[i] = GetParameterGenerator(type, cachedParameters[i], context).Generate(context);
        }

        return (TType?) constructor.Invoke(parameters);
    }

    /// <summary>
    /// Populates the provided instance with generated values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to populate.</typeparam>
    /// <param name="instance">The instance to populate.</param>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <remarks>
    /// Due to the boxing nature of value types, the <paramref name="instance"/> parameter is an object. This means the populated
    /// values are applied to the provided instance and not a copy.
    /// </remarks>
    public void PopulateInstance<TType>(object instance, AutoFakerContext context)
    {
        Type type = typeof(TType);

        CachedType cachedType = CacheService.Cache.GetCachedType(type);

        // Iterate the members and bind a generated value
        List<AutoMember> autoMembers = GetMembersToPopulate(cachedType);

        for (var i = 0; i < autoMembers.Count; i++)
        {
            AutoMember member = autoMembers[i];
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

            object? value = generator.Generate(context);

            if (value != null)
            {
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
                    else if (member.CachedType.IsCollection)
                    {
                        PopulateCollection(value, instance, member);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
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

    private CachedConstructor? GetConstructor(CachedType type)
    {
        if (_constructorsCache.TryGetValue(type, out CachedConstructor? cachedConstructor))
            return cachedConstructor;

        CachedConstructor[]? constructors = type.GetCachedConstructors();

        if (type.IsDictionary)
            return ResolveTypedConstructor(_genericDictionary, constructors);

        if (type.IsEnumerable)
            return ResolveTypedConstructor(_enumerable, constructors);

        for (var i = 0; i < constructors.Length; i++)
        {
            CachedConstructor constructor = constructors[i];

            if (constructor.GetCachedParameters().Length == 0)
            {
                return constructor;
            }
        }

        var rtnValue = constructors.Length > 0 ? constructors[0] : null;

        _constructorsCache.TryAdd(type, rtnValue);

        return rtnValue;
    }

    private static CachedConstructor? ResolveTypedConstructor(CachedType type, CachedConstructor[] constructors)
    {
        for (int i = 0; i < constructors.Length; i++)
        {
            CachedConstructor c = constructors[i];

            CachedParameter[] parameters = c.GetCachedParameters();

            if (parameters.Length != 1)
                continue;

            CachedParameter parameter = parameters[0];
            CachedType parameterType = parameter.CachedParameterType;

            if (!parameterType.IsGenericType)
                continue;

            CachedType? genericTypeDefinition = parameterType.GetCachedGenericTypeDefinition();

            if (genericTypeDefinition == type)
            {
                return c;
            }
        }

        return null;
    }

    private static IAutoFakerGenerator GetParameterGenerator(Type type, CachedParameter parameter, AutoFakerContext context)
    {
        context.Setup(type, parameter.CachedParameterType, parameter.Name);

        return AutoFakerGeneratorFactory.GetGenerator(context);
    }

    private List<AutoMember> GetMembersToPopulate(CachedType cachedType)
    {
        if (_autoMembersCache.TryGetValue(cachedType, out List<AutoMember>? members))
            return members;

        var autoMembers = new List<AutoMember>();

        PropertyInfo[] properties = cachedType.GetProperties()!;

        FieldInfo[]? fields = cachedType.GetFields();

        for (var i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];
            autoMembers.Add(new AutoMember(property));
        }

        for (var i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            autoMembers.Add(new AutoMember(field));
        }

        _autoMembersCache.TryAdd(cachedType, autoMembers);

        return autoMembers;
    }

    private static void PopulateDictionary(object value, object parent, AutoMember member)
    {
        if (value is not IDictionary dictionary)
            return;

        object? instance = member.Getter(parent);
        CachedType[] argTypes = member.CachedType.GetAddMethodArgumentTypes();
        CachedMethod? addMethod = GetAddMethod(member.CachedType, argTypes);

        if (instance == null || addMethod == null) 
            return;

        foreach (object? key in dictionary.Keys)
        {
            addMethod.Invoke(instance, [key, dictionary[key]]);
        }
    }

    private static void PopulateCollection(object value, object parent, AutoMember member)
    {
        if (value is not ICollection collection)
            return;

        object? instance = member.Getter(parent);
        CachedType[] argTypes = member.CachedType.GetAddMethodArgumentTypes();
        CachedMethod? addMethod = GetAddMethod(member.CachedType, argTypes);

        if (instance == null || addMethod == null) 
            return;

        foreach (object? item in collection)
        {
            addMethod.Invoke(instance, [item]);
        }
    }

    private static CachedMethod? GetAddMethod(CachedType cachedType, CachedType[] argTypes)
    {
        CachedMethod? method = cachedType.GetCachedMethod("Add", argTypes.ToTypes());

        if (method != null)
            return method;

        CachedType[]? interfaces = cachedType.GetCachedInterfaces();

        for (int i = 0; i < interfaces.Length; i++)
        {
            CachedMethod? interfaceMethod = GetAddMethod(interfaces[i], argTypes);

            if (interfaceMethod != null)
            {
                return interfaceMethod;
            }
        }

        return method;
    }
}