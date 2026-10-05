namespace hsbc_test_20260930.interfaces;

public interface SlidingWindowStatistics
{
    void add(int measurement);
    void subscribeForStatistics<T>(T subscriber, Action<Statistics> action) where T : class;
    Statistics getLatestStatistics();

    public interface Statistics
    {
        double getMean();
        double getMode();
        double getPctile(int pctile);
    }
}
