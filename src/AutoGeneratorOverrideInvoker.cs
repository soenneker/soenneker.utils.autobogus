using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Abstract;

namespace Soenneker.Utils.AutoBogus;

internal sealed class AutoGeneratorOverrideInvoker
    : IAutoGenerator
{
    internal AutoGeneratorOverrideInvoker(IAutoGenerator generator, List<AutoGeneratorOverride> overrides)
    {
        Generator = generator;
        Overrides = overrides;
    }

    internal IAutoGenerator Generator { get; }
    internal List<AutoGeneratorOverride> Overrides { get; }

    object IAutoGenerator.Generate(AutoGenerateContext context)
    {
        var overrideContext = new AutoGenerateOverrideContext(context);

        foreach (AutoGeneratorOverride? generatorOverride in Overrides)
        {
            // Check if an initialized instance is needed
            if (generatorOverride.Preinitialize && overrideContext.Instance == null)
            {
                overrideContext.Instance = Generator.Generate(context);
            }

            // Let each override apply updates to the instance
            generatorOverride.Generate(overrideContext);
        }      

        return overrideContext.Instance;
    }
}