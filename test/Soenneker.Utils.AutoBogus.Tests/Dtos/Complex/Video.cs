using System.Collections.Generic;
using System.IO;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

public class Video
{
    public List<MemoryStream> MemoryStreamsList { get; set; }

    public Stream[] StreamsArray { get; set; }

    internal string? Name { get; set; }

    internal string? Id;
}