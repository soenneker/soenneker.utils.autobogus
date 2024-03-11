using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Soenneker.Extensions.FieldInfo;
using Soenneker.Reflection.Cache.Constructors;
using Soenneker.Reflection.Cache.Extensions;
using Soenneker.Reflection.Cache.Fields;
using Soenneker.Reflection.Cache.Methods;
using Soenneker.Reflection.Cache.Parameters;
using Soenneker.Reflection.Cache.Properties;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus;

///<inheritdoc cref="IAutoFakerBinder"/>
public class AutoFakerBinder : IAutoFakerBinder
{
    private readonly AutoFakerConfig _autoFakerConfig;
    private readonly CacheService _cacheService;

    internal readonly GeneratorService GeneratorService;

    private readonly Dictionary<CachedType, List<AutoMember>> _autoMembersCache = [];
    private readonly Dictionary<CachedType, CachedConstructor> _constructorsCache = [];

    public AutoFakerBinder(AutoFakerConfig autoFakerConfig, CacheService? cacheService = null)
    {
        _autoFakerConfig = autoFakerConfig;

        if (cacheService != null)
            _cacheService = cacheService;
        else
            _cacheService = new CacheService();

        GeneratorService = new GeneratorService();
    }

    /// <summary>
    /// Creates an instance of <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to create.</typeparam>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <param name="cachedType"></param>
    /// <returns>The created instance of <typeparamref name="TType"/>.</returns>
    public TType? CreateInstance<TType>(AutoFakerContext context, CachedType cachedType)
    {
        CachedConstructor? constructor = GetConstructor(context.CachedType);

        if (context.RecursiveConstructorStack.Count(c => c == context.CachedType.CacheKey) >= _autoFakerConfig.RecursiveDepth)
        {
            context.RecursiveConstructorStack.Pop();
            return default;
        }

        context.RecursiveConstructorStack.Push(context.CachedType.CacheKey.Value);

        if (constructor == null)
            return default;

        CachedParameter[] cachedParameters = constructor.GetCachedParameters();

        if (cachedParameters.Length == 0)
        {
            return (TType?) constructor.Invoke();
        }

        var parameters = new object[cachedParameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i] = GetParameterGenerator(cachedType, cachedParameters[i], context).Generate(context);
        }

        return (TType?) constructor.Invoke(parameters);
    }

    /// <summary>
    /// Populates the provided instance with generated values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to populate.</typeparam>
    /// <param name="instance">The instance to populate.</param>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <param name="cachedType"></param>
    /// <remarks>
    /// Due to the boxing nature of value types, the <paramref name="instance"/> parameter is an object. This means the populated
    /// values are applied to the provided instance and not a copy.
    /// </remarks>
    public void PopulateInstance<TType>(object instance, AutoFakerContext context, CachedType cachedType)
    {
        // Iterate the members and bind a generated value
        List<AutoMember> autoMembers = GetMembersToPopulate(cachedType);
        context.RecursiveConstructorStack.Clear();

        PopulateMembers(instance, context, cachedType, autoMembers);
    }

    internal void PopulateMembers(object instance, AutoFakerContext context, CachedType cachedType, List<AutoMember> autoMembers)
    {
        // Iterate the members and bind a generated value
        for (var i = 0; i < autoMembers.Count; i++)
        {
            AutoMember member = autoMembers[i];

            // Check if the member has a skip config or the type has already been generated as a parent
            // If so skip this generation otherwise track it for use later in the object tree
            if (ShouldSkip(member, context))
            {
                continue;
            }

            context.Setup(cachedType, member.CachedType, member.Name);

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
                    else if (member.IsDictionary)
                    {
                        PopulateDictionary(value, instance, member);
                    }
                    else if (member.IsCollection)
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

    private bool ShouldSkip(AutoMember autoMember, AutoFakerContext context)
    {
        if (autoMember.ShouldSkip)
            return true;

        //check if tree depth is reached
        if (_autoFakerConfig.TreeDepth != null)
        {
            if (context.TypesStack.Count >= _autoFakerConfig.TreeDepth)
                return true;
        }

        if (context.TypesStack.Count < _autoFakerConfig.RecursiveDepth)
            return false;

        // Finally check if the recursive depth has been reached
        int typeCount = context.TypesStack.Count(c => c == autoMember.CachedType.CacheKey);

        if (typeCount >= _autoFakerConfig.RecursiveDepth)
            return true;

        return false;
    }

    private CachedConstructor? GetConstructor(CachedType cachedType)
    {
        if (_constructorsCache.TryGetValue(cachedType, out CachedConstructor? cachedConstructor))
            return cachedConstructor;

        CachedConstructor[]? constructors = cachedType.GetCachedConstructors();

        if (constructors == null)
            return null;

        if (cachedType.IsDictionary)
            return ResolveTypedConstructor(CachedTypeService.IDictionary.Value, constructors);

        if (cachedType.IsEnumerable)
            return ResolveTypedConstructor(CachedTypeService.IEnumerable.Value, constructors);

        for (var i = 0; i < constructors.Length; i++)
        {
            CachedConstructor constructor = constructors[i];

            if (constructor.GetCachedParameters().Length == 0)
            {
                return constructor;
            }
        }

        CachedConstructor? rtnValue = constructors.Length > 0 ? constructors[0] : null;

        _constructorsCache.TryAdd(cachedType, rtnValue);

        return rtnValue;
    }

    private static CachedConstructor? ResolveTypedConstructor(CachedType type, CachedConstructor[] constructors)
    {
        for (var i = 0; i < constructors.Length; i++)
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

    private static IAutoFakerGenerator GetParameterGenerator(CachedType type, CachedParameter parameter, AutoFakerContext context)
    {
        context.Setup(type, parameter.CachedParameterType, parameter.Name);

        return AutoFakerGeneratorFactory.GetGenerator(context);
    }

    internal List<AutoMember> GetMembersToPopulate(CachedType cachedType)
    {
        if (_autoMembersCache.TryGetValue(cachedType, out List<AutoMember>? members))
            return members;

        var autoMembers = new List<AutoMember>();

        CachedProperty[] cachedProperties = cachedType.GetCachedProperties()!;
        CachedField[]? cachedFields = cachedType.GetCachedFields();
        autoMembers.Capacity = cachedProperties.Length + (cachedFields?.Length ?? 0);

        for (var i = 0; i < cachedProperties.Length; i++)
        {
            CachedProperty cachedProperty = cachedProperties[i];
            autoMembers.Add(new AutoMember(cachedProperty, _cacheService, _autoFakerConfig));
        }

        if (cachedFields != null)
        {
            for (var i = 0; i < cachedFields.Length; i++)
            {
                CachedField cachedField = cachedFields[i];

                if (cachedField.FieldInfo.IsConstant())
                    continue;

                autoMembers.Add(new AutoMember(cachedField, _cacheService, _autoFakerConfig));
            }
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

        if (interfaces == null)
            return method;

        for (var i = 0; i < interfaces.Length; i++)
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