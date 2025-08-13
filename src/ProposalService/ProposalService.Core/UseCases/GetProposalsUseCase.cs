using ProposalService.Domain.Entities;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Ports.Outbound;

namespace ProposalService.Core.UseCases;

public class GetProposalsUseCase : IGetProposalsPort
{
    private readonly IProposalRepositoryPort _proposalRepository;

    public GetProposalsUseCase(IProposalRepositoryPort proposalRepository)
    {
        _proposalRepository = proposalRepository;
    }

    public async Task<IEnumerable<ProposalResult>> ExecuteAsync()
    {
        var proposals = await _proposalRepository.GetAllAsync();
        
        return proposals.Select(p => new ProposalResult(
            p.Id,
            p.CustomerName,
            p.CustomerEmail,
            p.InsuranceType,
            p.CoverageAmount,
            p.PremiumAmount,
            p.Status,
            p.CreatedAt,
            p.UpdatedAt,
            p.RejectionReason
        ));
    }
} 