using hsbc_test_20260930.interfaces;
using System.Collections;
using static hsbc_test_20260930.interfaces.ProbabilisticRandomGen;

namespace hsbc_test_20260930.q1;

/// <summary>
/// basic implementation where we accumulate the probability in a buffer,
/// take a random number [0..1] and look where this number should fit in the buffer.
/// we take then the associated value.
/// </summary>
public class ProbabilisticRandomGenImpl : ProbabilisticRandomGen
{
    private readonly NumAndProbability[] _thresholdData;
    private readonly Random _random = new();

    public ProbabilisticRandomGenImpl(List<NumAndProbability> inputData)
    {
        _thresholdData = new NumAndProbability[inputData.Count];

        // we will scale if the sum of all probabilities is not 1
        float sum = inputData.Sum(i => i.ProbabilityOfSample);

        // accumulate the probability in the buffer
        float acc = 0f;
        for(int i = 0; i < inputData.Count; ++i)
        {
            acc += inputData[i].ProbabilityOfSample / sum;

            // ensure no floating point error for the last entry
            if ( i == inputData.Count - 1 )
            {
                acc = 1.0f;
            }

            _thresholdData[i] = new(inputData[i].Number, acc);
        }
    }

    public int nextFromSample()
    {
        float rand = (float)_random.NextDouble();
        int index = Array.BinarySearch(_thresholdData, rand, new ProbabilisticClass());
        if ( index < 0 )
        {
            index = ~index;
        }
        return _thresholdData[index].Number;
    }

    public class ProbabilisticClass : IComparer
    {
        int IComparer.Compare(object? x, object? y)
        {
            return (x as NumAndProbability).ProbabilityOfSample.CompareTo((float)y);
        }
    }
}
