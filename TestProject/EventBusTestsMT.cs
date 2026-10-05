using FluentAssertions;
using hsbc_test_20260930.interfaces;
using hsbc_test_20260930.q2;

public class EventBusTestsMT
{
    private readonly EventBus _eventBus;

    public EventBusTestsMT()
    {
        // Remplacez par votre classe concrète (ex: MyEventBus)
        _eventBus = new EventBusMT();
    }

    // Classe de test d'exemple
    public record OrderCreatedEvent(Guid OrderId, decimal Amount);

    [Fact]
    public async Task AddSubscriber_ShouldReceiveEvent_WhenEventIsPublished()
    {
        // Arrange
        var tcs = new TaskCompletionSource<OrderCreatedEvent>();
        var expectedEvent = new OrderCreatedEvent(Guid.NewGuid(), 150.00m);

        _eventBus.addSubscriber<OrderCreatedEvent>(ev => tcs.SetResult(ev));

        // Act
        _eventBus.publishEvent(expectedEvent);

        // Assert
        // On attend la réponse, mais on met une sécurité (timeout de 2 secondes max)
        var delayTask = Task.Delay(TimeSpan.FromSeconds(2));
        var completedTask = await Task.WhenAny(tcs.Task, delayTask);

        // Si c'est le delay qui a fini en premier, c'est que le test a expiré
        completedTask.Should().Be(tcs.Task, "l'événement aurait dû être reçu avant le timeout");

        var receivedEvent = await tcs.Task;
        receivedEvent.Should().NotBeNull();
        receivedEvent.OrderId.Should().Be(expectedEvent.OrderId);
    }
}
