using System.Threading.Tasks;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Soenneker.Benchmarking.Extensions.Summary;
using Soenneker.Facts.Local;
using Soenneker.Facts.Manual;
using Soenneker.Tests.Benchmark;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

[Collection("Collection")]
public class GenerateRunner : BenchmarkTest
{
    public GenerateRunner(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    //[ManualFact]
    [LocalFact]
    public async ValueTask Generate()
    {
        Summary summary = BenchmarkRunner.Run<GenerateBenchmarks>(DefaultConf);

        await summary.OutputSummaryToLog(OutputHelper, CancellationToken);
    }

    //[ManualFact]
    [LocalFact]
    public async ValueTask GenerateT()
    {
        Summary summary = BenchmarkRunner.Run<GenerateTBenchmarks>(DefaultConf);

        await summary.OutputSummaryToLog(OutputHelper, CancellationToken);
    }

    //[ManualFact]
    [LocalFact]
    public async ValueTask Bogus()
    {
        Summary summary = BenchmarkRunner.Run<BogusBenchmarks>(DefaultConf);

        await summary.OutputSummaryToLog(OutputHelper, CancellationToken);
    }
}