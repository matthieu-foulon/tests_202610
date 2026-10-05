using FluentAssertions;
using hsbc_test_20260930.interfaces;
using hsbc_test_20260930.q2;

public class EventBusFilterTestsMT
{
    private readonly EventBus _eventBus;

    public EventBusFilterTestsMT()
    {
        _eventBus = new EventBusMT();
    }

    public record PaymentProcessedEvent(Guid PaymentId, bool IsSuccess);

    [Fact]
    public async Task AddSubscriberForFilteredEvents_ShouldReceiveEvent_WhenPredicateIsTrue()
    {
        // Arrange
        var tcs = new TaskCompletionSource<PaymentProcessedEvent>();
        var successfulPayment = new PaymentProcessedEvent(Guid.NewGuid(), true);

        // On ne veut que les paiements réussis
        _eventBus.addSubscriberForFilteredEvents<PaymentProcessedEvent>(
            action: ev => tcs.SetResult(ev),
            predicate: ev => ev.IsSuccess
        );

        // Act
        _eventBus.publishEvent(successfulPayment);

        // Assert
        // On attend la réponse, mais on met une sécurité (timeout de 2 secondes max)
        var delayTask = Task.Delay(TimeSpan.FromSeconds(2));
        var completedTask = await Task.WhenAny(tcs.Task, delayTask);

        // Si c'est le delay qui a fini en premier, c'est que le test a expiré
        completedTask.Should().Be(tcs.Task, "l'événement aurait dû être reçu avant le timeout");

        var receivedEvent = await tcs.Task;
        receivedEvent.Should().NotBeNull();
        receivedEvent.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AddSubscriberForFilteredEvents_ShouldNotReceiveEvent_WhenPredicateIsFalse()
    {
        // Arrange
        var tcs = new TaskCompletionSource<PaymentProcessedEvent>();
        var failedPayment = new PaymentProcessedEvent(Guid.NewGuid(), false);

        // On ne veut que les paiements réussis
        _eventBus.addSubscriberForFilteredEvents<PaymentProcessedEvent>(
            action: ev => tcs.SetResult(ev),
            predicate: ev => ev.IsSuccess
        );

        // Act
        _eventBus.publishEvent(failedPayment);

        // Assert
        // On attend la réponse, mais on met une sécurité (timeout de 2 secondes max)
        var delayTask = Task.Delay(TimeSpan.FromSeconds(2));
        var completedTask = await Task.WhenAny(tcs.Task, delayTask);

        // Si c'est le delay qui a fini en premier, c'est que le test a expiré
        completedTask.Should().Be(delayTask, "l'événement aurait dû être reçu avant le timeout");

        tcs.Task.IsCompleted.Should().BeFalse();
    }
}
