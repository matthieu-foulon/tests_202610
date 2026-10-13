using FluentAssertions;
using test_20260930.q4;

namespace TestProject;

public class HelpersTests
{
    public HelpersTests()
    {

    }

    public static IEnumerable<object[]> GetInputData()
    {
        yield return new object[] { Array.Empty<int>() };
        yield return new object[] { new int[] { -5, -5, 0, 3, 3, 3, 10, 10 } };
        yield return new object[] { new int[] { 1, 2, 3, 4 } };
        yield return new object[] { new int[] { 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42 } };
        yield return new object[] { new int[] { 12, 12, 12, 42, 42, 42, 42, 42, 42, 42, 42, 42 } };
    }


    [Theory]
    [MemberData(nameof(GetInputData))]
    public void ContructSortedArray_Check(int[] data)
    {
        SortedDictionary<int, int> dict = new();
        foreach ( int d in data )
        {
            if ( dict.TryGetValue(d, out int value) )
            {
                dict[d] = ++value;
            }
            else
            {
                dict[d] = 1;
            }
        }

        int[] result = Helpers.ContructSortedArray(dict, data.Length);
        result.Should().BeEquivalentTo(data, options => options.WithStrictOrdering());
    }
}
