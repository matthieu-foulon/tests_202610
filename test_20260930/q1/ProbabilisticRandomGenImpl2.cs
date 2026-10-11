using hsbc_test_20260930.interfaces;
using static hsbc_test_20260930.interfaces.ProbabilisticRandomGen;

namespace hsbc_test_20260930.q1;

/// <summary>
/// we still accumulated the probability into a buffer BUT
/// we are mapping a random number [0..1] to a unique array index
/// which contains the associated value, meaning a direct access when calling nextFromSample
/// </summary>
public class ProbabilisticRandomGenImpl2 : ProbabilisticRandomGen
{
    const int ArraySize = 1000000;
    private readonly int[] _filteredData = new int[ArraySize];

    public ProbabilisticRandomGenImpl2(List<NumAndProbability> inputData)
    {
        // we will scale if the sum of all probabilities is not 1
        float sum = inputData.Sum(i => i.ProbabilityOfSample);

        float acc = 0f;
        int startIndex = 0;
        for(int i = 0; i < inputData.Count; ++i)
        {
            acc += inputData[i].ProbabilityOfSample / sum;
            if ( i == inputData.Count - 1 )
            {
                acc = 1.0f;
            }

            int endIndex = (int)( acc * ArraySize );
            for( ; startIndex < endIndex; ++startIndex )
            {
                _filteredData[startIndex] = inputData[i].Number;
            }
        }
    }

    public int nextFromSample()
    {
        int index = Random.Shared.Next(ArraySize);
        return _filteredData[index];
    }
}
