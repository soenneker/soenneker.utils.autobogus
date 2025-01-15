using System.IO;
using System.Threading;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using Xunit;

namespace Soenneker.Utils.AutoBogus.Tests.Benchmarking;

public abstract class BenchmarkTest
{
    protected ManualConfig DefaultConf { get; }

    private readonly ITestOutputHelper _outputHelper;

    protected BenchmarkTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        DefaultConf = ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator);
        DefaultConf.SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
    }

    protected async System.Threading.Tasks.ValueTask OutputSummaryToLog(Summary summary, CancellationToken cancellationToken = default)
    {
        string[] logs = await File.ReadAllLinesAsync(summary.LogFilePath, cancellationToken);

        foreach (string? log in logs)
        {
            _outputHelper.WriteLine(log);
        }
    }
}