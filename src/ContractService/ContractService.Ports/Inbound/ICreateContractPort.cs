using ContractService.Ports.Inbound.Shared;

namespace ContractService.Ports.Inbound;

public interface ICreateContractPort
{
    Task<ContractResult> ExecuteAsync(CreateContractRequest request);
}

public record CreateContractRequest(Guid ProposalId); 