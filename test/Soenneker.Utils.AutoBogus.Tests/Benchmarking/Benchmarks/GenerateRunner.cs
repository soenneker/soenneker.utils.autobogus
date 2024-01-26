using System.Threading.Tasks;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Soenneker.Utils.AutoBogus.Tests.Facts;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

[Collection("Collection")]
public class GenerateRunner : BenchmarkTest
{
    public GenerateRunner(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [LocalFact]
    public async Task Generate()
    {
        Summary summary = BenchmarkRunner.Run<GenerateBenchmarks>(DefaultConf);

        await OutputSummaryToLog(summary);
    }

    [LocalFact]
    public async Task Bogus()
    {
        Summary summary = BenchmarkRunner.Run<BogusBenchmarks>(DefaultConf);

        await OutputSummaryToLog(summary);
    }
}
