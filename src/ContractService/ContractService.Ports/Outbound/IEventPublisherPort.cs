namespace ContractService.Ports.Outbound;

public interface IEventPublisherPort
{
    Task PublishAsync<T>(T @event) where T : class;
} 