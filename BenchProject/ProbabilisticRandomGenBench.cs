using BenchmarkDotNet.Attributes;
using hsbc_test_20260930.q1;
using static hsbc_test_20260930.interfaces.ProbabilisticRandomGen;

namespace BenchProject;

public class ProbabilisticRandomGenBench
{
    private readonly ProbabilisticRandomGenImplBasic v1;
    private readonly ProbabilisticRandomGenImpl v2;
    private readonly ProbabilisticRandomGenImpl v3;

    public ProbabilisticRandomGenBench()
    {
        List<NumAndProbability> probs = [new(0, 50), new(1, 200), new(2, 100)];

        v1 = new(probs);
        v2 = new(probs, 1000000);
        v3 = new(probs, 1024);
    }

    [Benchmark]
    public int V1() => v1.nextFromSample();

    [Benchmark]
    public int V2() => v2.nextFromSample();

    [Benchmark]
    public int V3() => v3.nextFromSample();
}
