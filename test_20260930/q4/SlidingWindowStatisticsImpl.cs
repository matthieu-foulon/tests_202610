using hsbc_test_20260930.interfaces;
using test_20260930.q4;
using static hsbc_test_20260930.interfaces.SlidingWindowStatistics;

namespace hsbc_test_20260930.q4;

public class SlidingWindowStatisticsImpl : SlidingWindowStatistics
{
    const string Topic = "statistics";

    private int _sum = 0;
    private int _count = 0;
    private SortedDictionary<int, int> _data = new();
    private readonly object _lock = new object();

    private readonly EventBusStr _eventBus;

    public SlidingWindowStatisticsImpl(EventBusStr eventBus)
    {
        _eventBus = eventBus;
    }

    public void add(int measurement)
    {
        lock ( _lock )
        {
            _sum += measurement;
            _count++;
            if ( _data.TryGetValue(measurement, out int value) )
            {
                _data[measurement] = ++value;
            }
            else
            {
                _data[measurement] = 1;
            }
        }

        _eventBus.publishEvent(Topic, getLatestStatistics());
    }

    public SlidingWindowStatistics.Statistics getLatestStatistics()
    {
        lock ( _lock )
        {
            double mean = (double)_sum / _count;
            double mode = _data.MaxBy(m => m.Value).Key;
            int[] data = Helpers.ContructSortedArray(_data, _count);

            return new StatisticsImpl(mean, mode, data);
        }
    }

    public void subscribeForStatistics(Action<Statistics> action)
    {
        _eventBus.addSubscriber(Topic, (o) => { if ( o is Statistics stats ) action(stats); });
    }

    public void subscribeWithFilterForStatistics(Action<Statistics> action, Func<Statistics, bool> predicate)
    {
        _eventBus.addSubscriberForFilteredEvents(
            Topic,
            (o) => { if ( o is Statistics stats ) action(stats); },
            (o) => { if ( o is Statistics stats ) return predicate(stats); else return false; }
        );
    }
}
