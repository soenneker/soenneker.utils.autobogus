using System.Numerics;

namespace Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

internal class TestClassWithNumber<T> where T : INumber<T>
{
    public T Number { get; set; }
}