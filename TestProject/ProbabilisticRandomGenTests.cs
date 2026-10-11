using FluentAssertions;
using hsbc_test_20260930.interfaces;
using hsbc_test_20260930.q1;

public class ProbabilisticRandomGenTests
{
    private static List<ProbabilisticRandomGen.NumAndProbability> samples =
    [
        new(Number: 1, ProbabilityOfSample: 0.70f),
        new(Number: 2, ProbabilityOfSample: 0.20f),
        new(Number: 3, ProbabilityOfSample: 0.10f)
    ];

    public static IEnumerable<object[]> GetImplementations()
    {
        yield return new object[] { new ProbabilisticRandomGenImpl(samples) };
        yield return new object[] { new ProbabilisticRandomGenImpl2(samples) };
        yield return new object[] { new ProbabilisticRandomGenImpl3(samples, 1024) };
    }


    [Theory]
    [MemberData(nameof(GetImplementations))]
    public void NextFromSample_ShouldRespectProbabilities_OverLargeSample(ProbabilisticRandomGen generator)
    {
        int totalIterations = 100_000_000;

        Dictionary<int, int> results = new();
        foreach ( var sample in samples )
        {
            results[sample.Number] = 0;
        }

        for ( int i = 0; i < totalIterations; i++ )
        {
            int number = generator.nextFromSample();
            results[number]++;
        }

        foreach(var result in results)
        {
            float freq = (float)result.Value / totalIterations;
            float freqToMatch = samples.First(x => x.Number == result.Key).ProbabilityOfSample;

            // check at +/- 0.2% (0.002f)
            freq.Should().BeApproximately(freqToMatch, 0.002f);
        }
    }
}
