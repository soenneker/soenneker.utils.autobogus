using System;
using System.Linq.Expressions;
using System.Reflection;
using Soenneker.Extensions.MemberInfo;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Config.Abstract;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;

namespace Soenneker.Utils.AutoBogus.Extensions;

/// <summary>
/// A class extending the config builder interfaces.
/// </summary>
public static class AutoConfigBuilderExtension
{
    /// <summary>
    /// Registers a binder type to use when generating values.
    /// </summary>
    /// <typeparam name="TBinder">The <see cref="IAutoFakerBinder"/> type to use.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerDefaultConfigBuilder WithBinder<TBinder>(this IAutoFakerDefaultConfigBuilder builder)
        where TBinder : AutoFakerBinder, new()
    {
        var binder = new TBinder();
        return builder.WithBinder(binder);
    }

    /// <summary>
    /// Registers a binder type to use when generating values.
    /// </summary>
    /// <typeparam name="TBinder">The <see cref="IAutoFakerBinder"/> type to use.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoGenerateConfigBuilder WithBinder<TBinder>(this IAutoGenerateConfigBuilder builder)
        where TBinder : AutoFakerBinder, new()
    {
        var binder = new TBinder();
        return builder.WithBinder(binder);
    }

    /// <summary>
    /// Registers a binder type to use when generating values.
    /// </summary>
    /// <typeparam name="TBinder">The <see cref="IAutoFakerBinder"/> type to use.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerConfigBuilder WithBinder<TBinder>(this IAutoFakerConfigBuilder builder)
        where TBinder : AutoFakerBinder, new()
    {
        var binder = new TBinder();
        return builder.WithBinder(binder);
    }

    /// <summary>
    /// Registers a type to skip when generating values.
    /// </summary>
    /// <typeparam name="TType">The type to skip.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerDefaultConfigBuilder WithSkip<TType>(this IAutoFakerDefaultConfigBuilder builder)
    {
        Type type = typeof(TType);
        return builder.WithSkip(type);
    }

    /// <summary>
    /// Registers a type to skip when generating values.
    /// </summary>
    /// <typeparam name="TType">The type to skip.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoGenerateConfigBuilder WithSkip<TType>(this IAutoGenerateConfigBuilder builder)
    {
        Type type = typeof(TType);
        return builder.WithSkip(type);
    }

    /// <summary>
    /// Registers a type to skip when generating values.
    /// </summary>
    /// <typeparam name="TType">The type to skip.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerConfigBuilder WithSkip<TType>(this IAutoFakerConfigBuilder builder)
    {
        Type type = typeof(TType);
        return builder.WithSkip(type);
    }

    /// <summary>
    /// Registers a member to skip for a given type when generating values.
    /// </summary>
    /// <typeparam name="TType">The parent type containing the member.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="member">The member to skip.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerDefaultConfigBuilder WithSkip<TType>(this IAutoFakerDefaultConfigBuilder builder, Expression<Func<TType, object>> member)
    {
        string? memberName = GetMemberName(member);
        return builder.WithSkip<TType>(memberName);
    }

    /// <summary>
    /// Registers a member to skip for a given type when generating values.
    /// </summary>
    /// <typeparam name="TType">The parent type containing the member.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="member">The member to skip.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoGenerateConfigBuilder WithSkip<TType>(this IAutoGenerateConfigBuilder builder, Expression<Func<TType, object>> member)
    {
        string? memberName = GetMemberName(member);
        return builder.WithSkip<TType>(memberName);
    }

    /// <summary>
    /// Registers a member to skip for a given type when generating values.
    /// </summary>
    /// <typeparam name="TType">The parent type containing the member.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="member">The member to skip.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerConfigBuilder WithSkip<TType>(this IAutoFakerConfigBuilder builder, Expression<Func<TType, object>> member)
    {
        string? memberName = GetMemberName(member);
        return builder.WithSkip<TType>(memberName);
    }

    /// <summary>
    /// Registers an override instance to use when generating values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to override.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="generator">A handler used to generate the override.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerDefaultConfigBuilder WithOverride<TType>(this IAutoFakerDefaultConfigBuilder builder, Func<AutoFakerOverrideContext, TType> generator)
    {
        var generatorOverride = new AutoFakerGeneratorTypeOverride<TType>(generator);
        return builder.WithOverride(generatorOverride);
    }

    /// <summary>
    /// Registers an override instance to use when generating values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to override.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="generator">A handler used to generate the override.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoGenerateConfigBuilder WithOverride<TType>(this IAutoGenerateConfigBuilder builder, Func<AutoFakerOverrideContext, TType> generator)
    {
        var generatorOverride = new AutoFakerGeneratorTypeOverride<TType>(generator);
        return builder.WithOverride(generatorOverride);
    }

    /// <summary>
    /// Registers an override instance to use when generating values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to override.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="generator">A handler used to generate the override.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerConfigBuilder WithOverride<TType>(this IAutoFakerConfigBuilder builder, Func<AutoFakerOverrideContext, TType> generator)
    {
        var generatorOverride = new AutoFakerGeneratorTypeOverride<TType>(generator);
        return builder.WithOverride(generatorOverride);
    }

    /// <summary>
    /// Registers an override instance to use when generating values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to override.</typeparam>
    /// <typeparam name="TValue">The member type to override.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="member">The member to override.</param>
    /// <param name="generator">A handler used to generate the override.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerDefaultConfigBuilder WithOverride<TType, TValue>(this IAutoFakerDefaultConfigBuilder builder, Expression<Func<TType, object>> member, Func<AutoFakerOverrideContext, TValue> generator)
    {
        string? memberName = GetMemberName(member);
        var generatorOverride = new AutoFakerGeneratorMemberOverride<TType, TValue>(memberName, generator);

        return builder.WithOverride(generatorOverride);
    }

    /// <summary>
    /// Registers an override instance to use when generating values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to override.</typeparam>
    /// <typeparam name="TValue">The member type to override.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="member">The member to override.</param>
    /// <param name="generator">A handler used to generate the override.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoGenerateConfigBuilder WithOverride<TType, TValue>(this IAutoGenerateConfigBuilder builder, Expression<Func<TType, object>> member, Func<AutoFakerOverrideContext, TValue> generator)
    {
        string? memberName = GetMemberName(member);
        var generatorOverride = new AutoFakerGeneratorMemberOverride<TType, TValue>(memberName, generator);

        return builder.WithOverride(generatorOverride);
    }

    /// <summary>
    /// Registers an override instance to use when generating values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to override.</typeparam>
    /// <typeparam name="TValue">The member type to override.</typeparam>
    /// <param name="builder">The current configuration builder instance.</param>
    /// <param name="member">The member to override.</param>
    /// <param name="generator">A handler used to generate the override.</param>
    /// <returns>The current configuration builder instance.</returns>
    public static IAutoFakerConfigBuilder WithOverride<TType, TValue>(this IAutoFakerConfigBuilder builder, Expression<Func<TType, object>> member, Func<AutoFakerOverrideContext, TValue> generator)
    {
        string? memberName = GetMemberName(member);
        var generatorOverride = new AutoFakerGeneratorMemberOverride<TType, TValue>(memberName, generator);

        return builder.WithOverride(generatorOverride);
    }

    private static string? GetMemberName<TType>(Expression<Func<TType, object>>? member)
    {
        if (member == null)
            return null;

        MemberExpression expression;

        if (member.Body is UnaryExpression unary)
        {
            expression = unary.Operand as MemberExpression;
        }
        else
        {
            expression = member.Body as MemberExpression;
        }

        if (expression != null)
        {
            MemberInfo memberInfo = expression.Member;

            if (memberInfo.IsField() || memberInfo.IsProperty())
            {
                return memberInfo.Name;
            }
        }

        return null;
    }
}