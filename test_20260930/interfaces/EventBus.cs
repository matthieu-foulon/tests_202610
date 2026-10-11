namespace hsbc_test_20260930.interfaces;

public interface EventBus
{
    void publishEvent<TEvent>(TEvent message) where TEvent : class;
    void addSubscriber<TEvent>(Action<TEvent> action) where TEvent : class;
    void addSubscriberForFilteredEvents<TEvent>(Action<TEvent> action, Func<TEvent, bool> predicate) where TEvent : class;
}

public interface EventBusStr
{
    void publishEvent(string topic, object message);
    void addSubscriber(string topic, Action<object> action);
    void addSubscriberForFilteredEvents(string topic, Action<object> action, Func<object, bool> predicate);
}
