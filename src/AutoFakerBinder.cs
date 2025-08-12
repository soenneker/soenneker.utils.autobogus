using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private readonly ConcurrentDictionary<CachedType, List<AutoMember>> _autoMembersCache = [];
    private readonly ConcurrentDictionary<CachedType, CachedConstructor> _constructorsCache = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsCompilerGeneratedBackingField(string name)
    {
        // Backing fields always start with '<', have a '>' before the suffix, and end with "k__BackingField"
        return name.Length > 17
               && name[0] == '<'
               && name.EndsWith("k__BackingField", StringComparison.Ordinal);
    }

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

        CachedConstructor? constructor = GetConstructor(cachedType);

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
        List<AutoMember>? autoMembers = GetMembersToPopulate(cachedType, context.CacheService, context.Config);
        context.RecursiveConstructorStack.Clear();

        PopulateMembers(instance, context, cachedType, autoMembers);
    }

    internal static void PopulateMembers(object instance, AutoFakerContext context, CachedType cachedType, List<AutoMember>? autoMembers)
    {
        if (autoMembers == null)
            return;

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
                if (!member.IsReadOnly)
                {
                    member.Setter.Invoke(instance, value);
                }
                else
                {
                    if (member.IsDictionary)
                    {
                        PopulateDictionary(value, instance, member);
                    }
                    else if (member.IsCollection)
                    {
                        PopulateCollection(value, instance, member);
                    }
                }
            }

            // Remove the current type from the type stack so siblings can be created
            context.TypesStack.Pop();
        }
    }

    private static bool ShouldSkip(AutoMember autoMember, AutoFakerContext context)
    {
        // Check if the member explicitly indicates it should be skipped.
        if (autoMember.ShouldSkip)
            return true;

        // Cache configuration values for better readability and efficiency.
        AutoFakerConfig config = context.Config;
        Stack<int> typesStack = context.TypesStack;
        int stackCount = typesStack.Count;

        // Handle special case: If TreeDepth is 0, all elements should be skipped.
        if (config.TreeDepth == 0)
            return true;

        // Check if the tree depth is reached.
        if (config.TreeDepth != null && stackCount >= config.TreeDepth)
            return true;

        // Handle special case: If RecursiveDepth is 0, all recursive elements should be skipped.
        if (config.RecursiveDepth == 0)
            return true;

        // Check if the stack count is below the recursive depth threshold.
        if (stackCount < config.RecursiveDepth)
            return false;

        // Inline logic to count matching types in the stack.
        if (autoMember.CachedType.CacheKey is { } cacheKeyValue)
        {
            var typeCount = 0;
            foreach (int type in typesStack)
            {
                if (type == cacheKeyValue)
                {
                    typeCount++;

                    if (typeCount >= config.RecursiveDepth)
                        return true; // Exit early if recursive depth is reached.
                }
            }
        }

        return false;
    }

    private CachedConstructor? GetConstructor(CachedType cachedType)
    {
        return _constructorsCache.GetOrAdd(cachedType, static ct =>
        {
            ReadOnlySpan<CachedConstructor> constructors = ct.GetCachedConstructors().AsSpan();

            if (constructors.IsEmpty)
                return null;

            // Handle dictionary special case
            if (ct.IsDictionary)
            {
                CachedConstructor? dictCtor = ResolveTypedConstructor(CachedTypeService.IDictionary.Value, constructors);
                if (dictCtor != null)
                    return dictCtor;
            }

            // Handle enumerable special case
            if (ct.IsEnumerable)
            {
                CachedConstructor? enumCtor = ResolveTypedConstructor(CachedTypeService.IEnumerable.Value, constructors);
                if (enumCtor != null)
                    return enumCtor;
            }

            CachedConstructor? constructorWithParameters = null;
            foreach (ref readonly CachedConstructor constructor in constructors)
            {
                if (!constructor.IsPublic || constructor.IsStatic)
                    continue;

                Span<CachedParameter> parameters = constructor.GetCachedParameters().AsSpan();
                if (parameters.IsEmpty)
                    return constructor; // Found perfect match

                var viable = true;
                foreach (ref readonly CachedParameter parameter in parameters)
                {
                    if (parameter.CachedParameterType.IsFunc)
                    {
                        viable = false;
                        break;
                    }
                }

                if (viable)
                    constructorWithParameters ??= constructor;
            }

            return constructorWithParameters;
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CachedConstructor? ResolveTypedConstructor(CachedType targetGenericDef, ReadOnlySpan<CachedConstructor> constructors)
    {
        for (int i = 0; i < constructors.Length; i++)
        {
            ref readonly CachedConstructor ctor = ref constructors[i];

            // Avoid array -> span churn each loop by working on the span directly
            ReadOnlySpan<CachedParameter> parameters = ctor.GetCachedParameters().AsSpan();

            // We only care about ctors with exactly one parameter
            if (parameters.Length != 1)
                continue;

            ref readonly CachedParameter param = ref parameters[0];
            CachedType paramType = param.CachedParameterType;

            // Cheap check first
            if (!paramType.IsGenericType)
                continue;

            // Compare the generic type definition with the target
            CachedType? gdef = paramType.GetCachedGenericTypeDefinition();

            if (gdef == targetGenericDef)
                return ctor;
        }

        return null;
    }

    private static IAutoFakerGenerator GetParameterGenerator(CachedType type, CachedParameter parameter, AutoFakerContext context)
    {
        context.Setup(type, parameter.CachedParameterType, parameter.Name);

        return AutoFakerGeneratorFactory.GetGenerator(context);
    }

    internal List<AutoMember>? GetMembersToPopulate(CachedType cachedType, CacheService cacheService, AutoFakerConfig autoFakerConfig)
    {
        // Try to retrieve cached members to avoid redundant processing
        if (_autoMembersCache.TryGetValue(cachedType, out List<AutoMember>? cachedMembers))
            return cachedMembers;

        // Fetch cached properties and fields from the cached type
        CachedProperty[]? cachedProperties = cachedType.GetCachedProperties();
        CachedField[]? cachedFields = cachedType.GetCachedFields();

        // Calculate capacity to minimize allocations and resize operations
        int totalCapacity = (cachedProperties?.Length ?? 0) + (cachedFields?.Length ?? 0);

        if (totalCapacity == 0)
            return null;

        var autoMembers = new List<AutoMember>(totalCapacity);

        // Process properties
        if (cachedProperties is { Length: > 0 })
        {
            for (var i = 0; i < cachedProperties.Length; i++)
            {
                ref readonly CachedProperty cachedProperty = ref cachedProperties[i];
                if (cachedProperty.IsDelegate || cachedProperty.IsEqualityContract)
                    continue;

                autoMembers.Add(new AutoMember(cachedProperty, cacheService, autoFakerConfig));
            }
        }

        // Process fields if present
        if (cachedFields is { Length: > 0 })
        {
            for (var i = 0; i < cachedFields.Length; i++)
            {
                ref readonly CachedField cachedField = ref cachedFields[i];

                // Skip constants, delegates, and backing fields
                if (cachedField.FieldInfo.IsConstant() || cachedField.IsDelegate || IsCompilerGeneratedBackingField(cachedField.FieldInfo.Name))
                    continue;

                autoMembers.Add(new AutoMember(cachedField, cacheService, autoFakerConfig));
            }
        }

        // Cache the processed members for future use
        _autoMembersCache.TryAdd(cachedType, autoMembers);

        return autoMembers;
    }

    private static void PopulateDictionary(object value, object parent, AutoMember member)
    {
        if (value is not IDictionary dictionary)
            return;

        object? instance = member.Getter(parent);
        if (instance is IDictionary { IsReadOnly: true })
            return;

        CachedType[] argTypes = member.CachedType.GetAddMethodArgumentTypes();
        CachedMethod? addMethod = GetAddMethod(member.CachedType, argTypes);
        if (instance == null || addMethod == null)
            return;

        var args = new object[2]; // reuse the same array

        IDictionaryEnumerator enumerator = dictionary.GetEnumerator(); // one pass, no second lookup
        while (enumerator.MoveNext())
        {
            DictionaryEntry entry = enumerator.Entry;
            args[0] = entry.Key;
            args[1] = entry.Value;
            addMethod.Invoke(instance, args);
        }
    }

    private static void PopulateCollection(object value, object parent, AutoMember member)
    {
        if (value is not ICollection collection)
            return;

        object? instance = member.Getter(parent);

        switch (instance)
        {
            case null:
                return;
            case Array destArray:
            {
                int itemsToCopy = Math.Min(destArray.Length, collection.Count);
                var i = 0;


                foreach (object? item in collection)
                {
                    if (i >= itemsToCopy)
                        break;

                    // Handles boxing/unboxing and element-type conversion internally
                    destArray.SetValue(item, i++);
                }

                return;
            }
            case IList {IsReadOnly: true}:
                return;
        }

        CachedType[] argTypes = member.CachedType.GetAddMethodArgumentTypes();
        CachedMethod? addMethod = GetAddMethod(member.CachedType, argTypes);

        if (addMethod == null)
            return;

        var args = new object[1];

        foreach (object? item in collection)
        {
            args[0] = item; // boxing avoidance
            addMethod.Invoke(instance, args);
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