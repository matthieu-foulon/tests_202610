using hsbc_test_20260930.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static hsbc_test_20260930.interfaces.SlidingWindowStatistics;

namespace hsbc_test_20260930.q4;

public class StatisticsImpl : Statistics
{
    private readonly double _mean;
    private readonly double _mode;
    private readonly int[] _data;

    public StatisticsImpl(double mean, double mode, int[] data)
    {
        _mean = mean;
        _mode = mode;
        _data = data;
    }

    public double getMean()
    {
        return _mean;
    }

    public double getMode()
    {
        return _mode;
    }

    public double getPctile(int pctile)
    {
        int index = (int)Math.Ceiling(( pctile / 100.0 ) * _data.Length) - 1;
        index = Math.Max(0, Math.Min(index, _data.Length - 1));

        return _data[index];
    }
}

public class SlidingWindowStatisticsImpl : SlidingWindowStatistics
{
    private int _sum = 0;
    private int _count = 0;
    private SortedDictionary<int, int> _data = new();

    public void add(int measurement)
    {
        _sum += measurement;
        _count++;
        if ( _data.ContainsKey(measurement) )
        {
            _data[measurement]++;
        }
        else
        {
            _data[measurement] = 1;
        }
    }

    public SlidingWindowStatistics.Statistics getLatestStatistics()
    {
        double mean = (double)_sum / _count;
        double mode = _data.MaxBy(m => m.Value).Key;
        int[] data = _data.SelectMany(e => Enumerable.Repeat(e.Key, e.Value)).ToArray();

        return new StatisticsImpl(mean, mode, data);
    }

    public void subscribeForStatistics<T>(T subscriber, Action<Statistics> action) where T : class
    {
        throw new NotImplementedException();
    }

    private static int[] ContructSortedArray(SortedDictionary<int, int> data)
    {
        int size = 0;
        foreach ( var e in data )
        {
            size += e.Value;
        }

        int[] result = new int[size];

        int index = 0;
        foreach ( var e in data )
        {
            for( var i = 0; i < e.Value; i++ )
            {
                result[index++] = e.Key;
            }
        }

        return result;
    }
}
