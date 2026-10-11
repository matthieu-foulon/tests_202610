using FluentAssertions;
using hsbc_test_20260930.interfaces;
using test_20260930.q2;

namespace TestProject;

public class EventBusStrMTTests
{
    private readonly EventBusStrMT _eventBus;

    public EventBusStrMTTests()
    {
        _eventBus = new EventBusStrMT();
    }

    [Fact]
    public void AddSubscriber_InlineLambda()
    {
        string topic = "main";

        _eventBus.addSubscriber(topic, _ => { });
        _eventBus.addSubscriber(topic, _ => { });

        _eventBus.SubsriberCount(topic).Should().Be(2);
    }

    [Fact]
    public void AddSubscriber_NamedLambda()
    {
        string topic = "main";

        Action<object> l = _ => { };

        _eventBus.addSubscriber(topic, l);
        _eventBus.addSubscriber(topic, l);

        _eventBus.SubsriberCount(topic).Should().Be(1);
    }
}
