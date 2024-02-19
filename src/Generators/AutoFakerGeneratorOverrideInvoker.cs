using System.Collections.Generic;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators;

internal sealed class AutoFakerGeneratorOverrideInvoker : IAutoFakerGenerator
{
    internal AutoFakerGeneratorOverrideInvoker(IAutoFakerGenerator generator, List<AutoFakerGeneratorOverride> overrides)
    {
        Generator = generator;
        Overrides = overrides;
    }

    private IAutoFakerGenerator Generator { get; }

    private List<AutoFakerGeneratorOverride> Overrides { get; }

    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        var overrideContext = new AutoFakerOverrideContext(context);

        foreach (AutoFakerGeneratorOverride generatorOverride in Overrides)
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