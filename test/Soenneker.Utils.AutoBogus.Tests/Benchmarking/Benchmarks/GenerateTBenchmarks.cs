using BenchmarkDotNet.Attributes;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

public class GenerateTBenchmarks
{
    private AutoFaker<Order> _autoFakerComplex = null!;
    private AutoFaker<string> _autoFakerString = null!;

    [GlobalSetup]
    public void Setup()
    {
        _autoFakerComplex = new AutoFaker<Order>();
        _autoFakerString = new AutoFaker<string>();
    }

    [Benchmark]
    public Order Generate_complex()
    {
        return _autoFakerComplex.Generate();
    }

    [Benchmark]
    public string Generate_string()
    {
        return _autoFakerString.Generate();
    }
}