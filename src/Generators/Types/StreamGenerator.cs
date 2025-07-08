using System.IO;
using System.Text;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators.Abstract;

namespace Soenneker.Utils.AutoBogus.Generators.Types;

internal sealed class StreamGenerator: IAutoFakerGenerator
{
    object IAutoFakerGenerator.Generate(AutoFakerContext context)
    {
        string word = context.Faker.Random.Word();
        byte[] byteArray = Encoding.UTF8.GetBytes(word);

        var memoryStream = new MemoryStream(byteArray); 

        // Set the position to the beginning of the stream.
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}