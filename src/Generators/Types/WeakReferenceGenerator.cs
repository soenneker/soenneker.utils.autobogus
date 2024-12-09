using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class WeakReferenceGenerator: IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        var generated = context.Generate<string>();

        try
        {
            var obj = new WeakReference(generated);
            return obj;
        }
        catch (Exception)
        {
            return null!;
        }
    }
}