using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace BenchProject;

internal class Program
{
    static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .WithSummaryStyle(BenchmarkDotNet.Reports.SummaryStyle.Default)
            .AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default);

        var summary = BenchmarkRunner.Run<ProbabilisticRandomGenBench>(config);
    }
}
