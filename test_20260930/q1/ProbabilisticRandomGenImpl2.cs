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
    private readonly int[] _filteredData = new int[1000001];
    private readonly Random _random = new();

    public ProbabilisticRandomGenImpl2(List<NumAndProbability> inputData)
    {
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

            int endIndex = (int)( acc * 1000000 );
            for( ; startIndex < endIndex; ++startIndex )
            {
                _filteredData[startIndex] = inputData[i].Number;
            }
        }
    }

    public int nextFromSample()
    {
        int index = (int)( _random.NextSingle() * 1000000 );
        return _filteredData[index];
    }
}
