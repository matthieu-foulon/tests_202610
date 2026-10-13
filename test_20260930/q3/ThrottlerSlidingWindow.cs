using hsbc_test_20260930.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static hsbc_test_20260930.interfaces.Throttler;

namespace hsbc_test_20260930.q3;

public class ThrottlerSlidingWindow : Throttler
{
    private readonly Lock _lock = new();
    private readonly RingBuffer<DateTime> _slidingWindow;
    private readonly int _maxTokens;
    private readonly TimeSpan _timeSpan;
    private Thread _thread;
    private CancellationTokenSource _cancellationTokenSource;
    private DateTime _startTime;
    private int _isStarted = 0;

    public event EventHandler? OnTokenAvailable;

    public ThrottlerSlidingWindow(int maxTokens, TimeSpan timeSpan, bool start = false)
    {
        _slidingWindow = new(maxTokens);
        _maxTokens = maxTokens;
        _timeSpan = timeSpan;
        if ( start )
        {
            Start();
        }
    }

    public ThrottleResult shouldProceed()
    {
        lock ( _lock )
        {
            DateTime now = DateTime.Now;

            Clean(now);

            if ( HasAvailableToken )
            {
                _slidingWindow.Put(now);
                return ThrottleResult.PROCEED;
            }
            else
            {
                return ThrottleResult.DO_NOT_PROCEED;
            }
        }
    }

    public void Start()
    {
        if ( Interlocked.CompareExchange(ref _isStarted, 1, 0) == 0 )
        {
            _cancellationTokenSource = new();
            _startTime = DateTime.Now;
            _thread = new(() =>
            {
                Update(_cancellationTokenSource.Token);
            });
            _thread.Start();
        }
    }

    public void Stop()
    {
        if ( Interlocked.CompareExchange(ref _isStarted, 0, 1) == 1 )
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _thread.Join();
        }
    }

    public int TokenCount
    {
        get
        {
            lock ( _lock )
            {
                return _maxTokens - _slidingWindow.Length;
            }
        }
    }

    private bool HasAvailableToken => _slidingWindow.Length < _maxTokens;

    private void Update(CancellationToken token)
    {
        while ( !token.IsCancellationRequested )
        {
            try
            {
                lock ( _lock )
                {
                    DateTime now = DateTime.Now;

                    Clean(now);

                    // only correct if one and only one subcriber
                    if ( OnTokenAvailable != null && HasAvailableToken )
                    {
                        _slidingWindow.Put(now);
                        OnTokenAvailable.Invoke(this, EventArgs.Empty);
                    }
                }

                Thread.Sleep(1);
            }
            catch ( OperationCanceledException )
            {
                break;
            }
        }
    }

    private void Clean(DateTime now)
    {
        DateTime timeThreshold = now - _timeSpan;

        // trash all data before timeThreshold
        while ( _slidingWindow.TryPeek(out var peek) && peek < timeThreshold )
        {
            _slidingWindow.TryGet(out var _);
        }
    }
}
