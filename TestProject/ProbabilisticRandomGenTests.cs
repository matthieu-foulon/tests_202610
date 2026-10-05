using FluentAssertions;
using hsbc_test_20260930.interfaces;
using hsbc_test_20260930.q1;

public class ProbabilisticRandomGenTests
{
    [Fact]
    public void NextFromSample_ShouldRespectProbabilities_OverLargeSample()
    {
        // Arrange
        // Configuration : 70% de chance d'avoir 1, 20% pour 2, 10% pour 3
        var samples = new List<ProbabilisticRandomGen.NumAndProbability>
        {
            new(Number: 1, ProbabilityOfSample: 0.70f),
            new(Number: 2, ProbabilityOfSample: 0.20f),
            new(Number: 3, ProbabilityOfSample: 0.10f)
        };

        // Remplacer par votre classe concrète
        ProbabilisticRandomGen generator = new ProbabilisticRandomGenImpl(samples);

        int totalIterations = 10_000;
        var results = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 } };

        // Act
        for ( int i = 0; i < totalIterations; i++ )
        {
            int number = generator.nextFromSample();
            results[number]++;
        }

        // Assert
        // Calcul des fréquences réelles
        float freq1 = (float)results[1] / totalIterations;
        float freq2 = (float)results[2] / totalIterations;
        float freq3 = (float)results[3] / totalIterations;

        // On s'attend à ce que le résultat soit proche à +/- 2% (0.02f)
        freq1.Should().BeApproximately(0.70f, 0.02f);
        freq2.Should().BeApproximately(0.20f, 0.02f);
        freq3.Should().BeApproximately(0.10f, 0.02f);
    }
}
