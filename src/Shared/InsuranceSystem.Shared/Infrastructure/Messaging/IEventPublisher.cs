namespace InsuranceSystem.Shared.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
} 