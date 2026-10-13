using hsbc_test_20260930.interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace test_20260930.q2;

public class EventBusStrMT : EventBusStr
{
    private interface ISubscription
    {
        void Handle(object message);
    }

    private class Subscription : ISubscription
    {
        private readonly Func<object, bool> _predicate;
        private readonly Action<object> _action;

        public Subscription(Func<object, bool> predicate, Action<object> action)
        {
            _predicate = predicate ?? ( _ => true );
            _action = action;
        }

        public void Handle(object message)
        {
            if ( _predicate(message) )
            {
                _action(message);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Subscription other
                && Delegate.Equals(_action, other._action)
                && Delegate.Equals(_predicate, other._predicate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_action, _predicate);
        }
    }

    // associate the topic to a set of subscribers, this, in some cases, allow us to have unique subscriber for each topic
    // unfortunately, this will not work in case of inline lambda, which c# contructs on the fly at each calling site.
    private readonly ConcurrentDictionary<string, HashSet<ISubscription>> _subscriptions = new();
    private readonly BlockingCollection<(string, object)> _messages = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Thread _thread;

    public EventBusStrMT()
    {
        _thread = new(() =>
        {
            Update(_cancellationTokenSource.Token);
        });
        _thread.Start();
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    public int SubsriberCount(string topic)
    {
        if ( !_subscriptions.TryGetValue(topic, out var list) )
        {
            return 0;
        }

        return list.Count;
    }

    private void Update(CancellationToken token)
    {
        try
        {
            var option = new ParallelOptions { MaxDegreeOfParallelism = 4 };
            Parallel.ForEach(_messages.GetConsumingEnumerable(token), option, message =>
            {
                Process(message);
            });
        }
        catch ( OperationCanceledException )
        {
        }
    }

    private void Process((string, object) message)
    {
        if ( !_subscriptions.TryGetValue(message.Item1, out var list) )
        {
            return;
        }

        // copy the list of subscribers into a temp
        // so we can process without disturbing addSubscriber* methods
        List<ISubscription> subscribers;
        lock (list)
        {
            subscribers = [.. list];
        }

        // not exception safe + check against slow consumer
        foreach ( var subscriber in subscribers )
        {
            var currentSubscriber = subscriber;
            Task.Run(() =>
            {
                try
                {
                    // we can't really do more in case of an unresponsive subscriber
                    // c# did remove killing a thread, so we hope this is not taking to much time
                    currentSubscriber.Handle(message.Item2);
                }
                catch
                {
                }
            });
        }
    }

    public void addSubscriber(string topic, Action<object> action)
    {
        addSubscriberForFilteredEvents(topic, action, null!);
    }

    public void addSubscriberForFilteredEvents(string topic, Action<object> action, Func<object, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(action);

        var subscribers = _subscriptions.GetOrAdd(topic, static _ => []);

        var subscription = new Subscription(predicate, action);

        lock ( subscribers )
        {
            subscribers.Add(subscription);
        }
    }

    public void publishEvent(string topic, object message)
    {
        _messages.Add((topic, message));
    }
}
