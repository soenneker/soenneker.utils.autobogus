using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Generators;

/// <summary>
/// Base class for providing custom generation logic that overrides the default behavior for specific types or members.
/// </summary>
/// <remarks>
/// Inherit from this class to create custom generators for specific scenarios. Overrides are evaluated during generation
/// and can provide custom logic for creating instances, setting specific values, or handling special cases.
/// </remarks>
public abstract class AutoFakerGeneratorOverride
{
    /// <summary>
    /// Gets a value indicating whether a pre-initialized instance is required before calling <see cref="Generate"/>.
    /// </summary>
    /// <remarks>
    /// When <see langword="true"/> (default), the generator will create a blank instance before calling <see cref="Generate"/>,
    /// allowing you to populate an existing instance. When <see langword="false"/>, you must create the instance yourself in <see cref="Generate"/>.
    /// </remarks>
    public virtual bool Preinitialize
    {
        get => true;
    }

    /// <summary>
    /// Determines whether this override should be used for the current generation request.
    /// </summary>
    /// <param name="context">The context containing information about the current generation request, including the type being generated and parent information.</param>
    /// <returns><see langword="true"/> if this override should handle the generation request; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// This method is called for each generation request to determine if this override applies. You can check the type,
    /// parent type, member name, or other context properties to decide whether to handle the request.
    /// </remarks>
    public abstract bool CanOverride(AutoFakerContext context);

    /// <summary>
    /// Generates the value for the current generation request using custom logic.
    /// </summary>
    /// <param name="context">The context containing information about the current generation request and the instance to populate (if <see cref="Preinitialize"/> is true).</param>
    /// <remarks>
    /// Implement this method to provide custom generation logic. If <see cref="Preinitialize"/> is <see langword="true"/>,
    /// the instance will already be created and you can populate it. Otherwise, you must create and assign the instance to <see cref="AutoFakerOverrideContext.Instance"/>.
    /// </remarks>
    public abstract void Generate(AutoFakerOverrideContext context);
}