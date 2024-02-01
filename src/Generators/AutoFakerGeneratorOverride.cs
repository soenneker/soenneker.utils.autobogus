using Soenneker.Utils.AutoBogus.Context;

namespace Soenneker.Utils.AutoBogus.Generators;

/// <summary>
/// A class for overriding generate requests.
/// </summary>
public abstract class AutoFakerGeneratorOverride
{
    /// <summary>
    /// Gets whether a pre-initialized instance is required. Defaults to true.
    /// </summary>
    public virtual bool Preinitialize
    {
        get => true;
    }

    /// <summary>
    /// Determines whether a generate request can be overridden.
    /// </summary>
    /// <param name="context">The <see cref="AutoFakerContext"/> instance for the current generate request.</param>
    /// <returns>true if the generate reqest can be overridden; otherwise false.</returns>
    public abstract bool CanOverride(AutoFakerContext context);

    /// <summary>
    /// Generates an override instance of a given type.
    /// </summary>
    /// <param name="context">The <see cref="AutoFakerContextOverride"/> instance for the current generate request.</param>
    public abstract void Generate(AutoFakerContextOverride context);
}