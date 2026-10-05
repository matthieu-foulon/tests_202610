using System;
using System.Collections.Generic;
using System.Text;

namespace hsbc_test_20260930.q3;

public class ThrottlerSlidingWindow
{
    private readonly Lock _lock = new();
    private readonly RingBuffer<DateTime> _slidingWindow;
    private readonly int _maxTokens;
    private readonly TimeSpan _timeSpan;
    private readonly Thread _thread;
    private DateTime _startTime;

    public event EventHandler? OnTokenAvailable;

    public ThrottlerSlidingWindow(int maxTokens, TimeSpan timeSpan)
    {
        _slidingWindow = new(maxTokens);
        _maxTokens = maxTokens;
        _timeSpan = timeSpan;
        _thread = new(() =>
        {
            Update();
        });
        Start();
    }

    public bool TryGetToken()
    {
        DateTime now = DateTime.Now;

        using var scope = _lock.EnterScope();

        Clean(now);

        if ( HasAvailableToken )
        {
            _slidingWindow.Put(now);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Start()
    {
        _startTime = DateTime.Now;
        _thread.Start();
    }

    private bool HasAvailableToken => _slidingWindow.Length < _maxTokens;

    private void Update()
    {
        DateTime now = DateTime.Now;

        using var scope = _lock.EnterScope();

        Clean(now);

        // someone subscribed to the event and is there at least one token available
        if ( OnTokenAvailable != null && HasAvailableToken )
        {
            _slidingWindow.Put(now);
            OnTokenAvailable.Invoke(this, EventArgs.Empty);
        }
    }

    private void Clean(DateTime now)
    {
        DateTime timeThreshold = now - _timeSpan;

        // trash all data before timeThreshold
        while ( !_slidingWindow.Empty && _slidingWindow.Peek() < timeThreshold )
        {
            _slidingWindow.Get();
        }
    }
}
