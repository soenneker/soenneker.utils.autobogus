using System;
using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Config.Abstract;

namespace Soenneker.Utils.AutoBogus.Abstract;

/// <summary>
/// An interface for invoking generate requests.
/// </summary>
public interface IAutoFaker
{
    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated instance.</returns>
    TType Generate<TType>(Action<IAutoGenerateConfigBuilder> configure = null);

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of instance to generate.</typeparam>
    /// <param name="count">The number of instances to generate.</param>
    /// <param name="configure">A handler to build the generate request configuration.</param>
    /// <returns>The generated collection of instances.</returns>
    List<TType> Generate<TType>(int count, Action<IAutoGenerateConfigBuilder> configure = null);
}