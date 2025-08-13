using ContractService.Ports.Inbound.Shared;

namespace ContractService.Ports.Inbound;

public interface IGetContractByProposalIdPort
{
    Task<ContractResult?> ExecuteAsync(Guid proposalId);
} 