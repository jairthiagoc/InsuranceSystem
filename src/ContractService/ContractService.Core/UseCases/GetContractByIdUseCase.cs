using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using ContractService.Ports.Outbound;

namespace ContractService.Core.UseCases;

public class GetContractByIdUseCase : IGetContractByIdPort
{
    private readonly IContractRepositoryPort _contractRepository;

    public GetContractByIdUseCase(IContractRepositoryPort contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<ContractResult?> ExecuteAsync(Guid id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);

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