using System.Threading.Tasks;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Soenneker.Benchmarking.Extensions.Summary;
using Soenneker.Tests.Benchmark;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking.Benchmarks;

public class GenerateRunner : BenchmarkTest
{
    public GenerateRunner() : base()
    {
    }

    [Skip("Manual")]
    //[LocalOnly]
    public async ValueTask Generate()
    {
        Summary summary = BenchmarkRunner.Run<GenerateBenchmarks>(DefaultConf);

        await summary.OutputSummaryToLog();
    }

    [Skip("Manual")]
    public async ValueTask GenerateT()
    {
        Summary summary = BenchmarkRunner.Run<GenerateTBenchmarks>(DefaultConf);

        await summary.OutputSummaryToLog();
    }

   [Skip("Manual")]
    public async ValueTask Bogus()
    {
        Summary summary = BenchmarkRunner.Run<BogusBenchmarks>(DefaultConf);

        await summary.OutputSummaryToLog();
    }
}
