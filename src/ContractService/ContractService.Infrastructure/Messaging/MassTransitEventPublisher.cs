using InsuranceSystem.Shared.Infrastructure.Messaging;
using MassTransit;

namespace ContractService.Infrastructure.Messaging;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
        => _publishEndpoint.Publish(@event, cancellationToken);
} 