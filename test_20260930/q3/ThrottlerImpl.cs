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
    private readonly Thread _thread;
    private double _tokens;
    DateTime _lastRefillTime;

    public ThrottlerImpl(int initTokens, int maxTokens, TimeSpan timeSpan)
    {
        _maxTokens = maxTokens;
        _refillRate = (double)maxTokens / timeSpan.Nanoseconds;
        _tokens = initTokens;
        _thread = new(() =>
        {
            Update();
        });
        Start();
    }

    private void Start()
    {
        _lastRefillTime = DateTime.Now;
        _thread.Start();
    }

    private void Update()
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
