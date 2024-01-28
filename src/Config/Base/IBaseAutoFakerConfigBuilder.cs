using System;
using Bogus;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Config.Base;

/// <summary>
/// An interface for building configurations.
/// </summary>
/// <typeparam name="TBuilder">The builder type.</typeparam>
public interface IBaseAutoFakerConfigBuilder<TBuilder>
{
    /// <summary>
    /// Registers the locale to use when generating values.
    /// </summary>
    /// <param name="locale">The locale to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithLocale(string locale);
    
    /// <summary>
    /// Registers the DateTimeKind to use when generating date and time values.
    /// </summary>
    /// <param name="dateTimeKind">The dateTimeKind to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithDateTimeKind(Func<AutoFakerContext, DateTimeKind> dateTimeKind);

    /// <summary>
    /// Registers the DateTimeKind to use when generating date and time values.
    /// </summary>
    /// <param name="dateTimeKind">The dateTimeKind to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithDateTimeKind(DateTimeKind dateTimeKind);

    /// <summary>
    /// Registers the number of items to generate for a collection.
    /// </summary>
    /// <param name="count">The repeat count to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithRepeatCount(int count);

    /// <summary>
    /// Registers the number of items to generate for a collection.
    /// </summary>
    /// <param name="count">The repeat count to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithRepeatCount(Func<AutoFakerContext, int> count);

    /// <summary>
    /// Registers the number of rows to generate in a <see cref="System.Data.DataTable"/>.
    /// </summary>
    /// <param name="count">The row count to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithDataTableRowCount(int count);

    /// <summary>
    /// Registers the number of rows to generate in a <see cref="System.Data.DataTable"/>.
    /// </summary>
    /// <param name="count">The row count to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithDataTableRowCount(Func<AutoFakerContext, int> count);

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
    /// Registers the depth to recursively generate.
    /// </summary>
    /// <param name="depth">The recursive depth to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithRecursiveDepth(Func<AutoFakerContext, int> depth);

    /// <summary>
    /// Registers the depth to generate the object tree
    /// </summary>
    /// <param name="depth">The depth to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithTreeDepth(Func<AutoFakerContext, int?> depth);

    /// <summary>
    /// Registers a binder instance to use when generating values.
    /// </summary>
    /// <param name="fakerBinder">The <see cref="IAutoFakerBinder"/> instance to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithBinder(AutoFakerBinder fakerBinder);

    /// <summary>
    /// Registers the <see cref="Faker"/> hub to use in underlying calls to Bogus.
    /// </summary>
    /// <param name="faker">The <see cref="Bogus.Faker"/> instance to use as the hub.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithFaker(Faker faker);

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
    /// <param name="generatorOverride">The <see cref="GeneratorOverride"/> instance to use.</param>
    /// <returns>The current configuration builder instance.</returns>
    TBuilder WithOverride(GeneratorOverride generatorOverride);
}