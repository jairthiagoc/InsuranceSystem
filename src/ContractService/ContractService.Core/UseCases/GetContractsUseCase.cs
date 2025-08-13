using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using ContractService.Ports.Outbound;

namespace ContractService.Core.UseCases;

public class GetContractsUseCase : IGetContractsPort
{
    private readonly IContractRepositoryPort _contractRepository;

    public GetContractsUseCase(IContractRepositoryPort contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<IEnumerable<ContractResult>> ExecuteAsync()
    {
        var contracts = await _contractRepository.GetAllAsync();

        return contracts.Select(c => new ContractResult(
            c.Id,
            c.ProposalId,
            c.ContractDate,
            c.ContractNumber,
            c.PremiumAmount,
            c.CreatedAt
        ));
    }
} 