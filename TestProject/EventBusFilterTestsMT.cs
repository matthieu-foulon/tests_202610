using FluentAssertions;
using hsbc_test_20260930.interfaces;
using test_20260930.q2;

namespace TestProject;

public class EventBusFilterTestsMT
{
    const string _topic = "topic";
    private readonly EventBusStr _eventBus;

    public EventBusFilterTestsMT()
    {
        _eventBus = new EventBusStrMT();
    }

    public record PaymentProcessedEvent(Guid PaymentId, bool IsSuccess);

    [Fact]
    public async Task AddSubscriberForFilteredEvents_ShouldReceiveEvent_WhenPredicateIsTrue()
    {
        var tcs = new TaskCompletionSource<PaymentProcessedEvent>();
        var successfulPayment = new PaymentProcessedEvent(Guid.NewGuid(), true);

        // filter on succeeded one
        _eventBus.addSubscriberForFilteredEvents(
            _topic,
            action: ev => tcs.SetResult((PaymentProcessedEvent)ev),
            predicate: ev => ( (PaymentProcessedEvent)ev ).IsSuccess
        );

        _eventBus.publishEvent(_topic, successfulPayment);

        // should take less than the timeout
        var delayTask = Task.Delay(TimeSpan.FromSeconds(2));
        var completedTask = await Task.WhenAny(tcs.Task, delayTask);

        completedTask.Should().Be(tcs.Task, "timeout");

        var receivedEvent = await tcs.Task;
        receivedEvent.Should().NotBeNull();
        receivedEvent.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AddSubscriberForFilteredEvents_ShouldNotReceiveEvent_WhenPredicateIsFalse()
    {
        var tcs = new TaskCompletionSource<PaymentProcessedEvent>();
        var failedPayment = new PaymentProcessedEvent(Guid.NewGuid(), false);

        // filter on succeeded one
        _eventBus.addSubscriberForFilteredEvents(
            _topic,
            action: ev => tcs.SetResult((PaymentProcessedEvent)ev),
            predicate: ev => ( (PaymentProcessedEvent)ev ).IsSuccess
        );

        _eventBus.publishEvent(_topic, failedPayment);

        var delayTask = Task.Delay(TimeSpan.FromSeconds(2));
        var completedTask = await Task.WhenAny(tcs.Task, delayTask);

        completedTask.Should().Be(delayTask, "timeout");

        tcs.Task.IsCompleted.Should().BeFalse();
    }
}
