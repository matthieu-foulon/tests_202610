namespace hsbc_test_20260930.interfaces;

public interface SlidingWindowStatistics
{
    void add(int measurement);
    void subscribeForStatistics(Action<Statistics> action);
    Statistics getLatestStatistics();

    public interface Statistics
    {
        double getMean();
        double getMode();
        double getPctile(int pctile);
    }
}
