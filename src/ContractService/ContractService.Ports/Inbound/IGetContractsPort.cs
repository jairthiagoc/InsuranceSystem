using ContractService.Ports.Inbound.Shared;

namespace ContractService.Ports.Inbound;

public interface IGetContractsPort
{
    Task<IEnumerable<ContractResult>> ExecuteAsync();
} 