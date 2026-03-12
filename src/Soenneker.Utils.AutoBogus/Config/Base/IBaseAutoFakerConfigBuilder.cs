using System;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config.Base;

/// <summary>
/// An interface for building configurations.
/// </summary>
/// <typeparam name="TBuilder">The builder type.</typeparam>
public interface IBaseAutoFakerConfigBuilder<out TBuilder>
{
    /// <summary>
    /// Sets the number of rows to generate when creating <see cref="System.Data.DataTable"/> instances.
    /// </summary>
    /// <param name="count">The number of rows to generate. Must be greater than 0.</param>
    /// <returns>The current configuration builder instance for method chaining.</returns>
    /// <remarks>
    /// This setting applies to all DataTable generation requests. If not specified, a default count will be used.
    /// </remarks>
    TBuilder WithDataTableRowCount(int count);

    /// <summary>
    /// Sets the maximum depth for recursively generating the same type within an object graph.
    /// </summary>
    /// <param name="depth">The maximum number of times the same type can appear in the generation path before stopping. A value of 0 disables recursive generation.</param>
    /// <returns>The current configuration builder instance for method chaining.</returns>
    /// <remarks>
    /// This prevents infinite loops when generating types that reference themselves (e.g., a Person with a Parent property of type Person).
    /// When the specified depth is reached for a type, further generation of that type is skipped.
    /// </remarks>
    TBuilder WithRecursiveDepth(int depth);

    /// <summary>
    /// Sets the maximum depth of the object tree to generate, controlling how many levels of nested objects are created.
    /// </summary>
    /// <param name="depth">The maximum depth of nested object generation. Use <see langword="null"/> for unlimited depth.</param>
    /// <returns>The current configuration builder instance for method chaining.</returns>
    /// <remarks>
    /// This limits the overall depth of the object graph, regardless of type. For example, with depth 2:
    /// Root object (depth 0) → Child object (depth 1) → Grandchild object (depth 2, will be skipped).
    /// This is useful for controlling the size and complexity of generated data structures.
    /// </remarks>
    TBuilder WithTreeDepth(int? depth);

    /// <summary>
    /// Configures the generator to skip generating values for properties or fields of the specified type, leaving them as their default values.
    /// </summary>
    /// <param name="type">The type to skip during generation. All members of this type will be left uninitialized.</param>
    /// <returns>The current configuration builder instance for method chaining.</returns>
    /// <remarks>
    /// This is useful for excluding complex types, expensive-to-generate types, or types that should remain null/default.
    /// Once a type is registered, all properties and fields of that type will be skipped during generation.
    /// </remarks>
    TBuilder WithSkip(Type type);

    /// <summary>
    /// Configures the generator to skip generating a value for a specific member (property or field) of the specified type.
    /// </summary>
    /// <typeparam name="TType">The type that contains the member to skip.</typeparam>
    /// <param name="memberName">The name of the property or field to skip. The member will be left as its default value.</param>
    /// <returns>The current configuration builder instance for method chaining.</returns>
    /// <remarks>
    /// This allows fine-grained control over which specific members are generated. Useful for excluding computed properties,
    /// read-only fields, or members that should remain uninitialized for testing purposes.
    /// </remarks>
    TBuilder WithSkip<TType>(string memberName);

    /// <summary>
    /// Configures the generator to skip generating a value for a specific member (property or field) of the specified type.
    /// </summary>
    /// <param name="type">The type that contains the member to skip.</param>
    /// <param name="memberName">The name of the property or field to skip. The member will be left as its default value.</param>
    /// <returns>The current configuration builder instance for method chaining.</returns>
    /// <remarks>
    /// This allows fine-grained control over which specific members are generated. Useful for excluding computed properties,
    /// read-only fields, or members that should remain uninitialized for testing purposes.
    /// </remarks>
    TBuilder WithSkip(Type type, string memberName);

    /// <summary>
    /// Registers a custom generator override that provides custom logic for generating specific types or members.
    /// </summary>
    /// <param name="autoFakerGeneratorOverride">The override instance that defines custom generation behavior.</param>
    /// <returns>The current configuration builder instance for method chaining.</returns>
    /// <remarks>
    /// Overrides allow you to provide custom generation logic for specific types or members. When an override matches
    /// a generation request (based on its <see cref="AutoFakerGeneratorOverride.CanOverride"/> method), it will be used
    /// instead of the default generation logic. Multiple overrides can be registered and will be evaluated in order.
    /// </remarks>
    TBuilder WithOverride(AutoFakerGeneratorOverride autoFakerGeneratorOverride);
}