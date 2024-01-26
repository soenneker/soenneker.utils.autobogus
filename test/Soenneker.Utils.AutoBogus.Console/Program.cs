using Soenneker.Utils.AutoBogus.Abstract;

namespace Soenneker.Utils.AutoBogus.Console;

public class Program
{
    public static void Main(string[] args)
    {
        IAutoFaker? faker = AutoFaker.Create();

        for (int i = 0; i < 10000; i++)
        {
            var order = faker.Generate<int>();
        }
    }
}