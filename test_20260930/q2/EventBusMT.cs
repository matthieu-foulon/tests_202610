using hsbc_test_20260930.interfaces;
using System.Collections.Concurrent;

namespace hsbc_test_20260930.q2;

public class EventBusMT : EventBus
{
    /// <summary>
    /// allow to hide further predicate and action real type
    /// behind a simple method.
    /// </summary>
    private interface ISubscription
    {
        void Handle(object message);
    }

    /// <summary>
    /// store callback and predicate given a event T.
    /// we consider a subscription unique if both the callback and the predicate
    /// are equal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class Subscription<T> : ISubscription where T : class
    {
        private readonly Func<T, bool> _predicate;
        private readonly Action<T> _action;

        public Subscription(Func<T, bool> predicate, Action<T> action)
        {
            _predicate = predicate ?? ( _ => true );
            _action = action;
        }

        public void Handle(object message)
        {
            if ( message is T typedMessage && _predicate(typedMessage) )
            {
                _action(typedMessage);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Subscription<T> other
                && Delegate.Equals(_action, other._action)
                && Delegate.Equals(_predicate, other._predicate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_action, _predicate);
        }
    }

    private readonly ConcurrentDictionary<Type, HashSet<ISubscription>> _subscriptions = new();
    private readonly BlockingCollection<object> _messages = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Thread _thread;

    public EventBusMT()
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

    private void Update(CancellationToken token)
    {
        try
        {
            foreach ( var message in _messages.GetConsumingEnumerable(token) )
            {
                Process(message);
            }
        }
        catch ( OperationCanceledException )
        {
        }
    }

    private void Process(object message)
    {
        if ( !_subscriptions.TryGetValue(message.GetType(), out var list) )
        {
            return;
        }

        // copy the list of subscribers into a temp
        // so we can process without disturbing addSubscriber* methods
        List<ISubscription> subscribers;
        lock ( list )
        {
            subscribers = [.. list];
        }

        foreach ( var subscriber in subscribers )
        {
            subscriber.Handle(message);
        }
    }

    public void publishEvent<TEvent>(TEvent message) where TEvent : class
    {
        _messages.Add(message);
    }

    public void addSubscriber<TEvent>(Action<TEvent> action) where TEvent : class
    {
        addSubscriberForFilteredEvents(action, null!);
    }

    public void addSubscriberForFilteredEvents<TEvent>(Action<TEvent> action, Func<TEvent, bool> predicate) where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(action);

        var subscribers = _subscriptions.GetOrAdd(typeof(TEvent), static _ => new());

        var subscription = new Subscription<TEvent>(predicate, action);

        lock ( subscribers )
        {
            subscribers.Add(subscription);
        }
    }
}
