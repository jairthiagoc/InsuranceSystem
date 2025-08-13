using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using ContractService.Ports.Outbound;

namespace ContractService.Core.UseCases;

public class GetContractByProposalIdUseCase : IGetContractByProposalIdPort
{
    private readonly IContractRepositoryPort _contractRepository;

    public GetContractByProposalIdUseCase(IContractRepositoryPort contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<ContractResult?> ExecuteAsync(Guid proposalId)
    {
        var contract = await _contractRepository.GetByProposalIdAsync(proposalId);
        
        if (contract == null)
            return null;

        return new ContractResult(
            contract.Id,
            contract.ProposalId,
            contract.ContractDate,
            contract.ContractNumber,
            contract.PremiumAmount,
            contract.CreatedAt
        );
    }
} 