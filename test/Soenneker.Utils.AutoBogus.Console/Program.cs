using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Console;

public class Program
{
    public static void Main(string[] args)
    {
        IAutoFaker? faker = AutoFaker.Create();

        for (int x = 0; x < 10000; x++)
        {
            var order = faker.Generate<Order>();
        }
    }
}