using System;
using System.Collections.Generic;
using System.Text;
using static hsbc_test_20260930.interfaces.SlidingWindowStatistics;

namespace test_20260930.q4;

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
