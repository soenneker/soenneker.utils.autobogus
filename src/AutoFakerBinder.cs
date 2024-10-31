using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
using Soenneker.Utils.AutoBogus.Utils;

namespace Soenneker.Utils.AutoBogus;

///<inheritdoc cref="IAutoFakerBinder"/>
public class AutoFakerBinder : IAutoFakerBinder
{
    internal readonly GeneratorService GeneratorService;

    private readonly Dictionary<CachedType, List<AutoMember>> _autoMembersCache = [];
    private readonly Dictionary<CachedType, CachedConstructor> _constructorsCache = [];

    public AutoFakerBinder()
    {
        GeneratorService = new GeneratorService();
    }

    public TType? CreateInstanceWithRecursionGuard<TType>(AutoFakerContext context, CachedType cachedType)
    {
        var recursionGuard = new RecursionGuard(context, cachedType.CacheKey.Value);

        if (recursionGuard.IsRecursive)
        {
            return default;
        }

        return CreateInstance<TType>(context, cachedType);
    }

    /// <summary>
    /// Creates an instance of <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to create.</typeparam>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <param name="cachedType"></param>
    /// <returns>The created instance of <typeparamref name="TType"/>.</returns>
    public virtual TType? CreateInstance<TType>(AutoFakerContext context, CachedType cachedType)
    {
        if (cachedType.IsAbstract || cachedType.IsInterface)
            return default;

        CachedConstructor? constructor = GetConstructor(context.CachedType);

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
        List<AutoMember> autoMembers = GetMembersToPopulate(cachedType, context.CacheService, context.Config);
        context.RecursiveConstructorStack.Clear();

        PopulateMembers(instance, context, cachedType, autoMembers);
    }

    internal static void PopulateMembers(object instance, AutoFakerContext context, CachedType cachedType, List<AutoMember> autoMembers)
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

    private static bool ShouldSkip(AutoMember autoMember, AutoFakerContext context)
    {
        if (autoMember.ShouldSkip)
            return true;

        //check if tree depth is reached
        if (context.Config.TreeDepth != null)
        {
            if (context.TypesStack.Count >= context.Config.TreeDepth)
                return true;
        }

        if (context.TypesStack.Count < context.Config.RecursiveDepth)
            return false;

        // Finally check if the recursive depth has been reached
        int typeCount = CountMatchingType(context.TypesStack, autoMember.CachedType.CacheKey);

        if (typeCount >= context.Config.RecursiveDepth)
            return true;

        return false;
    }

    private static int CountMatchingType(Stack<int> typesStack, int? cacheKey)
    {
        var typeCount = 0;

        foreach (int item in typesStack)
        {
            if (item == cacheKey)
            {
                typeCount++;
            }
        }

        return typeCount;
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

        CachedConstructor? constructorWithParameters = null;

        for (var i = 0; i < constructors.Length; i++)
        {
            CachedConstructor constructor = constructors[i];

            // Not going to support private constructors right now, and can't invoke a static one neither
            if (!constructor.IsPublic || constructor.IsStatic)
                continue;

            CachedParameter[] parameters = constructor.GetCachedParameters();

            if (parameters.Length == 0)
            {
                _constructorsCache.TryAdd(cachedType, constructor);
                return constructor;
            }

            var constructorIsViable = true;

            for (var j = 0; j < parameters.Length; j++)
            {
                CachedParameter parameter = parameters[j];

                if (parameter.CachedParameterType.IsFunc)
                {
                    constructorIsViable = false;
                    break;
                }
            }

            if (!constructorIsViable)
                continue;

            constructorWithParameters ??= constructor;
        }

        if (constructorWithParameters != null)
        {
            _constructorsCache.TryAdd(cachedType, constructorWithParameters);
            return constructorWithParameters;
        }

        return null;
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

    internal List<AutoMember> GetMembersToPopulate(CachedType cachedType, CacheService cacheService, AutoFakerConfig autoFakerConfig)
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

            if (cachedProperty.IsDelegate)
                continue;

            if (cachedProperty.IsEqualityContract)
                continue;

            autoMembers.Add(new AutoMember(cachedProperty, cacheService, autoFakerConfig));
        }

        if (cachedFields != null)
        {
            for (var i = 0; i < cachedFields.Length; i++)
            {
                CachedField cachedField = cachedFields[i];

                if (cachedField.FieldInfo.IsConstant())
                    continue;

                if (cachedField.IsDelegate)
                    continue;

                if (cachedField.FieldInfo.Name.Contains("k__BackingField"))
                    continue;

                autoMembers.Add(new AutoMember(cachedField, cacheService, autoFakerConfig));
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

        if (instance is IDictionary {IsReadOnly: true})
            return;

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

        if (instance is IList {IsReadOnly: true})
            return;

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