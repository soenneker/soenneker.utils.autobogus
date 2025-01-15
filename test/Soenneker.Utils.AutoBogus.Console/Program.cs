using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Tests.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var faker = new AutoFaker();

        for (var i = 0; i < 100000; i++)
        {
            var order = faker.Generate<Order>();
        }
    }
}