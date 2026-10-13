using FluentAssertions;
using hsbc_test_20260930.interfaces;
using test_20260930.q2;

namespace TestProject;

public class EventBusTestsMT
{
    const string _topic = "topic";
    private readonly EventBusStr _eventBus;

    public EventBusTestsMT()
    {
        _eventBus = new EventBusStrMT();
    }

    public record OrderCreatedEvent(Guid OrderId, decimal Amount);

    [Fact]
    public async Task AddSubscriber_ShouldReceiveEvent_WhenEventIsPublished()
    {
        var tcs = new TaskCompletionSource<OrderCreatedEvent>();
        var expectedEvent = new OrderCreatedEvent(Guid.NewGuid(), 150.00m);

        _eventBus.addSubscriber(_topic, ev => tcs.SetResult((OrderCreatedEvent)ev));

        _eventBus.publishEvent(_topic, expectedEvent);

        var delayTask = Task.Delay(TimeSpan.FromSeconds(2));
        var completedTask = await Task.WhenAny(tcs.Task, delayTask);

        completedTask.Should().Be(tcs.Task, "timeout");

        var receivedEvent = await tcs.Task;
        receivedEvent.Should().NotBeNull();
        receivedEvent.OrderId.Should().Be(expectedEvent.OrderId);
    }

    [Fact]
    public async Task AddSubscriber_ShouldReceiveEvent_Timeout()
    {
        var tcs = new TaskCompletionSource<OrderCreatedEvent>();
        var expectedEvent = new OrderCreatedEvent(Guid.NewGuid(), 150.00m);

        _eventBus.addSubscriber(_topic, ev => tcs.SetResult((OrderCreatedEvent)ev));

        var delayTask = Task.Delay(TimeSpan.FromSeconds(2));
        var completedTask = await Task.WhenAny(tcs.Task, delayTask);

        completedTask.Should().Be(delayTask, "timeout");

        tcs.Task.IsCompleted.Should().BeFalse();
    }
}
