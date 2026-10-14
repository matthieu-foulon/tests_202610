using FluentAssertions;
using hsbc_test_20260930.q4;
using test_20260930.q2;

namespace TestProject;

public class SlidingWindowStatisticsImplTests
{
    [Fact]
    public void Simple_GetLatestStatistics()
    {
        EventBusStrMT eventBus = new();
        SlidingWindowStatisticsImpl impl = new(eventBus);

        int[] values = [3, 4, 5, 5, 6, 7, 7, 8, 8, 8, 9, 10, 10, 11, 11, 12, 12, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 18, 18, 19, 20];
        double mean = (double)values.Sum() / values.Length;
        foreach ( int i in values )
        {
            impl.add(i);
        }

        var stats = impl.getLatestStatistics();
        stats.getMean().Should().BeApproximately(mean, 0.01);
        stats.getMode().Should().Be(12);
        stats.getPctile(25).Should().Be(8);
        stats.getPctile(50).Should().Be(12);
        stats.getPctile(75).Should().Be(15);
        stats.getPctile(90).Should().Be(18);
    }
}
