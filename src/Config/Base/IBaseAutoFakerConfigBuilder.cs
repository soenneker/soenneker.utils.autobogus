using System;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config.Base;

/// <summary>
/// An interface for building configurations.
/// </summary>
/// <typeparam name="TBuilder">The builder type.</typeparam>
public interface IBaseAutoFakerConfigBuilder<TBuilder>
{
    /// <summary>
    /// Registers the number of rows to generate in a <see cref="System.Data.DataTable"/>.
    /// </summary>
    /// <param name="count">The row count to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithDataTableRowCount(int count);

    /// <summary>
    /// Registers the depth to recursively generate.
    /// </summary>
    /// <param name="depth">The recursive depth to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithRecursiveDepth(int depth);

    /// <summary>
    /// Registers the depth of the object tree to generate
    /// </summary>
    /// <param name="depth">The depth to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithTreeDepth(int? depth);

    /// <summary>
    /// Registers a type to skip when generating values.
    /// </summary>
    /// <param name="type">The type to skip.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithSkip(Type type);

    /// <summary>
    /// Registers a member to skip for a given type when generating values.
    /// </summary>
    /// <typeparam name="TType">The parent type containing the member.</typeparam>
    /// <param name="memberName">The name of the member to skip.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithSkip<TType>(string memberName);

    /// <summary>
    /// Registers a member to skip for a given type when generating values.
    /// </summary>
    /// <param name="type">The parent type containing the member.</param>
    /// <param name="memberName">The name of the member to skip.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithSkip(Type type, string memberName);

    /// <summary>
    /// Registers an override instance to use when generating values.
    /// </summary>
    /// <param name="autoFakerGeneratorOverride">The <see cref="AutoFakerGeneratorOverride"/> instance to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithOverride(AutoFakerGeneratorOverride autoFakerGeneratorOverride);
}