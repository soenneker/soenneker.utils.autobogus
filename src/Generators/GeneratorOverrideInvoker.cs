using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class GeneratorOverrideInvoker
    : IAutoFakerGenerator
{
    internal GeneratorOverrideInvoker(IAutoFakerGenerator generator, List<GeneratorOverride> overrides)
    {
        Generator = generator;
        Overrides = overrides;
    }

    internal IAutoFakerGenerator Generator { get; }
    internal List<GeneratorOverride> Overrides { get; }

    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        var overrideContext = new AutoFakerContextOverride(context);

        foreach (GeneratorOverride? generatorOverride in Overrides)
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