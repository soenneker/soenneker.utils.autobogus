using System;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class ExceptionGenerator : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        return new Exception(context.Faker.Lorem.Sentence());
    }
}