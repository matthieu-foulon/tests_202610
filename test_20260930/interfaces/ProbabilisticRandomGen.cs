namespace hsbc_test_20260930.interfaces;

public interface ProbabilisticRandomGen
{
    public int nextFromSample();

    public record NumAndProbability(int Number, float ProbabilityOfSample);
}
