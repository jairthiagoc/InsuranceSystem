using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Ports.Outbound;

namespace ProposalService.Core.UseCases;

public class GetProposalsByStatusUseCase : IGetProposalsByStatusPort
{
    private readonly IProposalRepositoryPort _proposalRepository;

    public GetProposalsByStatusUseCase(IProposalRepositoryPort proposalRepository)
    {
        _proposalRepository = proposalRepository;
    }

    public async Task<IEnumerable<ProposalResult>> ExecuteAsync(ProposalStatus status)
    {
        var proposals = await _proposalRepository.GetByStatusAsync(status);
        
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