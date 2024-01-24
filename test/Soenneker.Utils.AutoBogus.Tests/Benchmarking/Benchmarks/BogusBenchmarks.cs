using BenchmarkDotNet.Attributes;
using Bogus;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

public class BogusBenchmarks
{
    private Faker _faker = default!;

    [GlobalSetup]
    public void Setup()
    {
        _faker = new Faker();
    }

    [Benchmark(Baseline = true)]
    public string Bogus_string()
    {
       return _faker.Vehicle.Vin();
    }

    [Benchmark]
    public int Bogus_int()
    {
        return _faker.Random.Int();
    }
}
