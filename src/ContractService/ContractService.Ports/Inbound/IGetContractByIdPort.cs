using ContractService.Ports.Inbound.Shared;

namespace ContractService.Ports.Inbound;

public interface IGetContractByIdPort
{
    Task<ContractResult?> ExecuteAsync(Guid id);
} 