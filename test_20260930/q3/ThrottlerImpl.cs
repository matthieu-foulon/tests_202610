using hsbc_test_20260930.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace hsbc_test_20260930.q3;

public class ThrottlerImpl : Throttler
{
    private object _lock = new();
    private readonly double _maxTokens;
    private readonly double _refillRate;
    private Thread _thread;
    private CancellationTokenSource _cancellationTokenSource;
    private double _tokens;
    private DateTime _lastRefillTime;
    private int _isStarted = 0;

    /// <summary>
    ///
    /// </summary>
    /// <param name="initTokens"></param>
    /// <param name="maxTokens"></param>
    /// <param name="timeSpan"></param>
    /// <param name="start"></param>
    public ThrottlerImpl(int initTokens, int maxTokens, TimeSpan timeSpan, bool start = false)
    {
        _maxTokens = maxTokens;
        _refillRate = (double)maxTokens / timeSpan.TotalNanoseconds;
        _tokens = initTokens < maxTokens ? initTokens : maxTokens;
        if ( start )
        {
            Start();
        }
    }

    public void Start()
    {
        if ( Interlocked.CompareExchange(ref _isStarted, 1, 0) == 0 )
        {
            _cancellationTokenSource = new();
            _lastRefillTime = DateTime.Now;
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

    private void Update(CancellationToken token)
    {
        while ( !token.IsCancellationRequested )
        {
            try
            {
                lock ( _lock )
                {
                    DateTime now = DateTime.Now;
                    double elapsed = ( now - _lastRefillTime ).TotalNanoseconds;
                    _lastRefillTime = now;

                    _tokens = Math.Min(_maxTokens, _tokens + elapsed * _refillRate);

                    if ( _tokens > 1.0 )
                    {
                        _tokens -= 1.0;
                        notifyWhenCanProceed();
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

    public int TokenCount
    {
        get
        {
            lock ( _lock )
            {
                return (int)_tokens;
            }
        }
    }

    public virtual void notifyWhenCanProceed()
    {
    }

    public Throttler.ThrottleResult shouldProceed()
    {
        lock ( _lock )
        {
            if ( _tokens >= 1.0 )
            {
                _tokens -= 1.0;
                return Throttler.ThrottleResult.PROCEED;
            }
            else
            {
                return Throttler.ThrottleResult.DO_NOT_PROCEED;
            }
        }
    }
}
