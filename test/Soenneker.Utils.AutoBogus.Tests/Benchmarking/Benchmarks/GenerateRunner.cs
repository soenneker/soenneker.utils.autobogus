using System.Threading.Tasks;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Soenneker.Facts.Manual;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

[Collection("Collection")]
public class GenerateRunner : BenchmarkTest
{
    public GenerateRunner(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [ManualFact]
    public async Task Generate()
    {
        Summary summary = BenchmarkRunner.Run<GenerateBenchmarks>(DefaultConf);

        await OutputSummaryToLog(summary);
    }

    [ManualFact]
    public async Task GenerateT()
    {
        Summary summary = BenchmarkRunner.Run<GenerateTBenchmarks>(DefaultConf);

        await OutputSummaryToLog(summary);
    }

    [ManualFact]
    public async Task Bogus()
    {
        Summary summary = BenchmarkRunner.Run<BogusBenchmarks>(DefaultConf);

        await OutputSummaryToLog(summary);
    }
}
