using BenchmarkDotNet.Attributes;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Complex;
using Soenneker.Utils.AutoBogus.Tests.Dtos.Simple;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

public class GenerateBenchmarks
{
    private IAutoFaker _faker = default!;

    [GlobalSetup]
    public void Setup()
    {
        _faker = AutoFaker.Create();
    }

    [Benchmark(Baseline = true)]
    public Order Generate_complex()
    {
        return _faker.Generate<Order>();
    }

    [Benchmark]
    public TestClassWithSingleProperty<int> Generate_simple_reference()
    {
       return _faker.Generate<TestClassWithSingleProperty<int>>();
    }

    [Benchmark]
    public int Generate_simple_value()
    {
        return _faker.Generate<int>();
    }
}