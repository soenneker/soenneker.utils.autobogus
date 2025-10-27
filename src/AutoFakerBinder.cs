﻿using Soenneker.Extensions.FieldInfo;
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
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Soenneker.Utils.AutoBogus;

///<inheritdoc cref="IAutoFakerBinder"/>
public class AutoFakerBinder : IAutoFakerBinder
{
    internal readonly GeneratorService GeneratorService;

    private readonly ConcurrentDictionary<CachedType, List<AutoMember>> _autoMembersCache = [];
    private readonly ConcurrentDictionary<CachedType, CachedConstructor> _constructorsCache = [];

    private const string _backingSuffix = "k__BackingField";

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
            return (TType?)constructor.Invoke();
        }

        var parameters = new object[cachedParameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i] = GetParameterGenerator(cachedType, cachedParameters[i], context).Generate(context);
        }

        return (TType?)constructor.Invoke(parameters);
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
        // Fast path: check the cache first.
        if (_constructorsCache.TryGetValue(cachedType, out CachedConstructor? cachedConstructor))
            return cachedConstructor;

        ReadOnlySpan<CachedConstructor> constructors = cachedType.GetCachedConstructors().AsSpan();

        if (constructors.IsEmpty)
            return null;

        // Handle specific type scenarios first for early exits.
        if (cachedType.IsDictionary)
        {
            cachedConstructor = ResolveTypedConstructor(CachedTypeService.IDictionary.Value, constructors);
            if (cachedConstructor != null)
            {
                _constructorsCache.TryAdd(cachedType, cachedConstructor);
                return cachedConstructor;
            }
        }

        if (cachedType.IsEnumerable)
        {
            cachedConstructor = ResolveTypedConstructor(CachedTypeService.IEnumerable.Value, constructors);
            if (cachedConstructor != null)
            {
                _constructorsCache.TryAdd(cachedType, cachedConstructor);
                return cachedConstructor;
            }
        }

        CachedConstructor? constructorWithParameters = null;

        foreach (ref readonly CachedConstructor constructor in constructors)
        {
            // Skip private or static constructors.
            if (!constructor.IsPublic || constructor.IsStatic)
                continue;

            ReadOnlySpan<CachedParameter> parameters = constructor.GetCachedParameters().AsSpan();

            // If parameterless, cache and return immediately.
            if (parameters.IsEmpty)
            {
                _constructorsCache.TryAdd(cachedType, constructor);
                return constructor;
            }

            // Check if all parameters are valid.
            var constructorIsViable = true;
            foreach (ref readonly CachedParameter parameter in parameters)
            {
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

        // Cache and return the constructor with parameters, if found.
        if (constructorWithParameters != null)
        {
            _constructorsCache.TryAdd(cachedType, constructorWithParameters);
            return constructorWithParameters;
        }

        return null;
    }

    private static CachedConstructor? ResolveTypedConstructor(CachedType type, ReadOnlySpan<CachedConstructor> constructors)
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

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    internal List<AutoMember>? GetMembersToPopulate(CachedType cachedType, CacheService cacheService, AutoFakerConfig autoFakerConfig)
    {
        if (_autoMembersCache.TryGetValue(cachedType, out List<AutoMember>? cachedMembers))
            return cachedMembers;

        // Fetch arrays (could be null)
        CachedProperty[]? cachedProperties = cachedType.GetCachedProperties();
        CachedField[]? cachedFields = cachedType.GetCachedFields();

        int propsLen = cachedProperties?.Length ?? 0;
        int fieldsLen = cachedFields?.Length ?? 0;
        int totalCapacity = propsLen + fieldsLen;

        if (totalCapacity == 0)
            return null;

        // Build into a pooled buffer, then bulk-copy into List<T> once.
        AutoMember[] buffer = ArrayPool<AutoMember>.Shared.Rent(totalCapacity);
        int w = 0;

        // Local copies for JIT friendliness
        CachedType ct = cachedType;
        CacheService svc = cacheService;
        AutoFakerConfig cfg = autoFakerConfig;

        // ----- Properties -----
        if (propsLen != 0)
        {
            CachedProperty[] props = cachedProperties!;
            for (int i = 0; i < props.Length; i++)
            {
                // ref readonly avoids defensive copies for structs
                ref readonly CachedProperty p = ref props[i];

                if (p.IsDelegate || p.IsEqualityContract)
                    continue;

                buffer[w++] = new AutoMember(p, ct, svc, cfg);
            }
        }

        // ----- Fields -----
        if (fieldsLen != 0)
        {
            CachedField[] fields = cachedFields!;
            for (int i = 0; i < fields.Length; i++)
            {
                ref readonly CachedField f = ref fields[i];

                // Fast reject constants & delegates first
                if (f.FieldInfo.IsConstant() || f.IsDelegate)
                    continue;

                string name = f.FieldInfo.Name;
                if (name.Length >= _backingSuffix.Length && name.EndsWith(_backingSuffix, StringComparison.Ordinal))
                    continue;

                buffer[w++] = new AutoMember(f, ct, svc, cfg);
            }
        }

        // Materialize result list
        List<AutoMember> result;
        if (w == 0)
        {
            result = [];
        }
        else
        {
            result = new List<AutoMember>(w);
            result.AddRange(buffer.AsSpan(0, w)); // bulk add (no per-item Add overhead)
        }

        // Return buffer
        Array.Clear(buffer, 0, w); // ensure references are cleared before returning to pool
        ArrayPool<AutoMember>.Shared.Return(buffer, clearArray: false);

        // Cache result (even if empty list)
        _autoMembersCache.TryAdd(cachedType, result);

        return result;
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
            case IList { IsReadOnly: true }:
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