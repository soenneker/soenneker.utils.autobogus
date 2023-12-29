namespace Soenneker.Utils.AutoBogus.Tests.Playground.Dtos;

public sealed class Units
{
    public Units(short size, Measure measure)
    {
        Size = size;
        Measure = measure;
    }

    public short Size { get; }
    public Measure Measure { get; }
}