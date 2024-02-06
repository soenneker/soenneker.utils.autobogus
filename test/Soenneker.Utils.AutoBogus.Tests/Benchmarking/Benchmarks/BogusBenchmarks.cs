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

    [Benchmark]
    public string Bogus_string()
    {
       return _faker.Random.Word();
    }

    [Benchmark]
    public int Bogus_int()
    {
        return _faker.Random.Int();
    }

    [Benchmark]
    public Faker Bogus_ctor()
    {
        return new Faker();
    }
}
