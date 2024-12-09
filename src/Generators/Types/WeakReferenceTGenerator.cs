using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Extensions;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class WeakReferenceGenerator<TType> : IAutoFakerGenerator where TType : class
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        var generated = context.Generate<TType>();

        try
        {
            var obj = new WeakReference<TType>(generated);
            return obj;
        }
        catch (Exception)
        {
            return null!;
        }
    }
}