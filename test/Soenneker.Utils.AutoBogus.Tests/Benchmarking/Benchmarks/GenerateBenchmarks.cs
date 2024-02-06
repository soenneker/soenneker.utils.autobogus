using BenchmarkDotNet.Attributes;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

public class GenerateBenchmarks
{
    private IAutoFaker _autoFaker = default!;

    [GlobalSetup]
    public void Setup()
    {
        _autoFaker = new AutoFaker();
    }

    [Benchmark]
    public int Generate_int()
    {
        return _autoFaker.Generate<int>();
    }

    [Benchmark]
    public string Generate_string()
    {
        return _autoFaker.Generate<string>();
    }

    [Benchmark]
    public Order Generate_complex()
    {
        return _autoFaker.Generate<Order>();
    }
}