using BenchmarkDotNet.Attributes;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

public class GenerateBenchmarks
{
    private IAutoFaker _autoFaker = default!;

    [GlobalSetup]
    public void Setup()
    {
        _autoFaker = AutoFaker.Create();
    }

    [Benchmark(Baseline = true)]
    public Order Generate_complex()
    {
        return _autoFaker.Generate<Order>();
    }

    [Benchmark]
    public TestClassWithSingleProperty<int> Generate_simple_reference()
    {
       return _autoFaker.Generate<TestClassWithSingleProperty<int>>();
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
}