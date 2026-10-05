using hsbc_test_20260930.interfaces;
using System.Collections.Concurrent;

namespace hsbc_test_20260930.q2;

public class EventBusST : EventBus
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

    private readonly Dictionary<Type, HashSet<ISubscription>> _subscriptions = new();

    public void publishEvent<TEvent>(TEvent message) where TEvent : class
    {
        if ( !_subscriptions.TryGetValue(message.GetType(), out var subscribers) )
        {
            return;
        }

        foreach ( var subscriber in subscribers )
        {
            subscriber.Handle(message);
        }
    }

    public void addSubscriber<TEvent>(Action<TEvent> action) where TEvent : class
    {
        addSubscriberForFilteredEvents(action, null!);
    }

    public void addSubscriberForFilteredEvents<TEvent>(Action<TEvent> action, Func<TEvent, bool> predicate) where TEvent : class
    {
        var subscription = new Subscription<TEvent>(predicate, action);

        if ( _subscriptions.TryGetValue(typeof(TEvent), out var subscriptions) )
        {
            subscriptions.Add(subscription);
        }
        else
        {
            _subscriptions[typeof(TEvent)] = [subscription];
        }
    }
}
