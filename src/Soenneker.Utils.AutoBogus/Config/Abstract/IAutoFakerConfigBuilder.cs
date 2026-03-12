using Soenneker.Utils.AutoBogus.Config.Base;

namespace Soenneker.Utils.AutoBogus.Config.Abstract;

/// <summary>
/// An interface for building configurations for fakers.
/// </summary>
public interface IAutoFakerConfigBuilder : IBaseAutoFakerConfigBuilder<IAutoFakerConfigBuilder>
{
    /// <summary>
    /// Specifies constructor arguments to use when creating instances of types that require constructor parameters.
    /// </summary>
    /// <param name="args">The arguments to pass to the type's constructor. These will be used when the generator needs to instantiate types with parameterized constructors.</param>
    /// <returns>The current configuration builder instance for method chaining.</returns>
    /// <remarks>
    /// This is useful when you need to provide specific values for constructor parameters during generation.
    /// The arguments will be matched to constructor parameters by position and type compatibility.
    /// </remarks>
    IAutoFakerConfigBuilder WithArgs(params object[] args);

    IAutoFakerConfigBuilder WithArgs(object? arg0);

    IAutoFakerConfigBuilder WithArgs(object? arg0, object? arg1);

    IAutoFakerConfigBuilder WithArgs(object? arg0, object? arg1, object? arg2);

    IAutoFakerConfigBuilder WithArgs(object? arg0, object? arg1, object? arg2, object? arg3);
}