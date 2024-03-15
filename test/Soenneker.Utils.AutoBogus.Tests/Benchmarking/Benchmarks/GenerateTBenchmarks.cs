using BenchmarkDotNet.Attributes;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

public class GenerateTBenchmarks
{
    private AutoFaker<Order> _autoFaker = default!;

    [GlobalSetup]
    public void Setup()
    {
        _autoFaker = new AutoFaker<Order>();
    }

    [Benchmark]
    public Order Generate_complex()
    {
        return _autoFaker.Generate();
    }
}