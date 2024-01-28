using System;
using System.Reflection;
using Bogus;
using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Abstract;

/// <summary>
/// An interface for binding generated instances.
/// </summary>
public interface IAutoFakerBinder : IBinder
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to create.</typeparam>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <returns>The created instance.</returns>
    TType? CreateInstance<TType>(AutoFakerContext context);

    object? CreateInstance(AutoFakerContext context, Type type);

    /// <summary>
    /// Populates the provided instance with generated values.
    /// </summary>
    /// <typeparam name="TType">The type of instance to populate.</typeparam>
    /// <param name="instance">The instance to populate.</param>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the generate request.</param>
    /// <param name="members">An optional collection of members to populate. If null, all writable instance members are populated.</param>
    /// <remarks>
    /// Due to the boxing nature of value types, the <paramref name="instance"/> parameter is an object. This means the populated
    /// values are applied to the provided instance and not a copy.
    /// </remarks>
    void PopulateInstance<TType>(object instance, AutoFakerContext context, MemberInfo[]? members = null);

    void PopulateInstance(object instance, AutoFakerContext context, Type type, MemberInfo[]? members = null);
}