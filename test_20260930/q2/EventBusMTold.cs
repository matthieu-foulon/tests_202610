#if false

using hsbc_test_20260930.interfaces;
using System.Collections.Concurrent;

namespace hsbc_test_20260930.q2;

public class EventBusMTold : EventBus
{
    private interface ISubscription
    {
        void Handle(object message);
    }

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
    }

    private class SafeSubscriptionToken : IDisposable
    {
        private Action _unsubscribe;
        public SafeSubscriptionToken(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }
        public void Dispose()
        {
            CleanUp();
            GC.SuppressFinalize(this);
        }
        ~SafeSubscriptionToken()
        {
            CleanUp();
        }
        private void CleanUp()
        {
            var action = Interlocked.Exchange(ref _unsubscribe, null!);
            action?.Invoke();
        }
    }

    private readonly ConcurrentDictionary<Type, List<ISubscription>> _subscriptions = new();
    private readonly BlockingCollection<object> _messages = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Thread _thread;

    public EventBusMTold()
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

    public IDisposable addSubscriber<TEvent>(Action<TEvent> action) where TEvent : class
    {
        return addSubscriberForFilteredEvents(action, null!);
    }

    public IDisposable addSubscriberForFilteredEvents<TEvent>(Action<TEvent> action, Func<TEvent, bool> predicate) where TEvent : class
    {
        var subscribers = _subscriptions.GetOrAdd(typeof(TEvent), _ => new());

        var subscription = new Subscription<TEvent>(predicate, action);

        lock ( subscribers )
        {
            subscribers.Add(subscription);
        }

        return new SafeSubscriptionToken(() => { lock ( subscribers ) { subscribers.Remove(subscription); } });
    }
}

#endif