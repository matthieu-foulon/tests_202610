using FluentAssertions;
using hsbc_test_20260930.interfaces;
using hsbc_test_20260930.q2;

public class EventBusTests
{
    private readonly EventBus _eventBus;

    public EventBusTests()
    {
        // Remplacez par votre classe concrète (ex: MyEventBus)
        _eventBus = new EventBusST();
    }

    // Classe de test d'exemple
    public record OrderCreatedEvent(Guid OrderId, decimal Amount);

    [Fact]
    public void AddSubscriber_ShouldReceiveEvent_WhenEventIsPublished()
    {
        // Arrange
        OrderCreatedEvent? receivedEvent = null;
        var expectedEvent = new OrderCreatedEvent(Guid.NewGuid(), 150.00m);

        _eventBus.addSubscriber<OrderCreatedEvent>(ev => receivedEvent = ev);

        // Act
        _eventBus.publishEvent(expectedEvent);

        // Assert
        receivedEvent.Should().NotBeNull();
        receivedEvent.Should().BeEquivalentTo(expectedEvent);
    }
}
