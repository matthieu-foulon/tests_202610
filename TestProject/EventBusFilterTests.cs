using FluentAssertions;
using hsbc_test_20260930.interfaces;
using hsbc_test_20260930.q2;

public class EventBusFilterTests
{
    private readonly EventBus _eventBus;

    public EventBusFilterTests()
    {
        _eventBus = new EventBusST();
    }

    public record PaymentProcessedEvent(Guid PaymentId, bool IsSuccess);

    [Fact]
    public void AddSubscriberForFilteredEvents_ShouldReceiveEvent_WhenPredicateIsTrue()
    {
        // Arrange
        PaymentProcessedEvent? receivedEvent = null;
        var successfulPayment = new PaymentProcessedEvent(Guid.NewGuid(), true);

        // On ne veut que les paiements réussis
        _eventBus.addSubscriberForFilteredEvents<PaymentProcessedEvent>(
            action: ev => receivedEvent = ev,
            predicate: ev => ev.IsSuccess
        );

        // Act
        _eventBus.publishEvent(successfulPayment);

        // Assert
        receivedEvent.Should().NotBeNull();
        receivedEvent.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void AddSubscriberForFilteredEvents_ShouldNotReceiveEvent_WhenPredicateIsFalse()
    {
        // Arrange
        PaymentProcessedEvent? receivedEvent = null;
        var failedPayment = new PaymentProcessedEvent(Guid.NewGuid(), false);

        // On ne veut que les paiements réussis
        _eventBus.addSubscriberForFilteredEvents<PaymentProcessedEvent>(
            action: ev => receivedEvent = ev,
            predicate: ev => ev.IsSuccess
        );

        // Act
        _eventBus.publishEvent(failedPayment);

        // Assert
        receivedEvent.Should().BeNull(); // Le filtre a dû bloquer l'événement
    }
}
