using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Abstract;

/// <summary>
/// Defines the contract for creating instances and populating them with generated values.
/// </summary>
/// <remarks>
/// The binder is responsible for the core generation logic: instantiating types (including handling constructors with parameters)
/// and populating properties and fields with generated values. It manages recursion detection and respects configuration settings.
/// </remarks>
public interface IAutoFakerBinder 
{
    /// <summary>
    /// Creates a new instance of the specified type, handling constructor selection and parameter generation.
    /// </summary>
    /// <typeparam name="TType">The type of instance to create.</typeparam>
    /// <param name="context">The generation context containing configuration, faker instance, and type information.</param>
    /// <param name="cachedType">The cached reflection information for the type to instantiate.</param>
    /// <returns>A new instance of <typeparamref name="TType"/> if creation succeeds; otherwise, <see langword="default"/>.</returns>
    /// <remarks>
    /// This method selects an appropriate constructor (preferring parameterless, then public constructors with generatable parameters),
    /// generates values for constructor parameters, and creates the instance. Abstract types and interfaces return <see langword="default"/>.
    /// </remarks>
    TType? CreateInstance<TType>(AutoFakerContext context, CachedType cachedType);

    /// <summary>
    /// Creates a new instance of the specified type with protection against infinite recursion in constructor calls.
    /// </summary>
    /// <typeparam name="TType">The type of instance to create.</typeparam>
    /// <param name="context">The generation context containing configuration, faker instance, and type information.</param>
    /// <param name="cachedType">The cached reflection information for the type to instantiate.</param>
    /// <returns>A new instance of <typeparamref name="TType"/> if creation succeeds and recursion is not detected; otherwise, <see langword="default"/>.</returns>
    /// <remarks>
    /// This method prevents infinite loops when types have constructors that reference themselves (e.g., a Person constructor that takes a Person parameter).
    /// If the same type is detected in the constructor call stack, creation is aborted and <see langword="default"/> is returned.
    /// </remarks>
    TType? CreateInstanceWithRecursionGuard<TType>(AutoFakerContext context, CachedType cachedType);

    /// <summary>
    /// Populates all writable properties and fields of the provided instance with generated values.
    /// </summary>
    /// <typeparam name="TType">The type of instance being populated.</typeparam>
    /// <param name="instance">The instance to populate. For value types, this is boxed, but the populated values are still applied to the original instance.</param>
    /// <param name="context">The generation context containing configuration, faker instance, and type information.</param>
    /// <param name="cachedType">The cached reflection information for the type being populated.</param>
    /// <remarks>
    /// This method iterates through all properties and fields of the instance and generates appropriate values for each.
    /// It respects configuration settings such as skip lists, recursion depth, tree depth, and custom overrides.
    /// For read-only collections and dictionaries, items are added using their Add methods.
    /// Due to the boxing nature of value types, the <paramref name="instance"/> parameter is an object, but the populated
    /// values are applied directly to the provided instance, not a copy.
    /// </remarks>
    void PopulateInstance<TType>(object instance, AutoFakerContext context, CachedType cachedType);
}