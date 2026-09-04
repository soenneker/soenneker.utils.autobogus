using System.IO;
using System.Text;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

/// <inheritdoc cref="IAutoFakerGenerator" />
internal sealed class StreamGenerator : IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        string word = context.Faker.Random.Word();
        byte[] byteArray = Encoding.UTF8.GetBytes(word);

        return new MemoryStream(byteArray);
    }
}
