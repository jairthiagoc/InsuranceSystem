using ProposalService.Ports.Outbound;
using MassTransit;

namespace ProposalService.Adapters.Outbound.Messaging;

public class MassTransitEventPublisher : IEventPublisherPort
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<T>(T @event) where T : class
        => _publishEndpoint.Publish(@event);
} 