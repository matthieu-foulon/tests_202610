using BenchmarkDotNet.Attributes;
using hsbc_test_20260930.q1;
using static hsbc_test_20260930.interfaces.ProbabilisticRandomGen;

namespace BenchProject;

public class ProbabilisticRandomGenBench
{
    private readonly ProbabilisticRandomGenImpl v1;
    private readonly ProbabilisticRandomGenImpl2 v2;
    private readonly ProbabilisticRandomGenImpl3 v3;
    private readonly ProbabilisticRandomGenImpl3 v4;

    public ProbabilisticRandomGenBench()
    {
        List<NumAndProbability> probs = [new(0, 50), new(1, 200), new(2, 100)];

        v1 = new(probs);
        v2 = new(probs);
        v3 = new(probs, 1000000);
        v4 = new(probs, 1024);
    }

    [Benchmark]
    public int V1() => v1.nextFromSample();

    [Benchmark]
    public int V2() => v2.nextFromSample();

    [Benchmark]
    public int V3() => v3.nextFromSample();

    [Benchmark]
    public int V4() => v4.nextFromSample();
}
