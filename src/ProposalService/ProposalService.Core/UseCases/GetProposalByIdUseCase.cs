using ProposalService.Domain.Entities;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Ports.Outbound;

namespace ProposalService.Core.UseCases;

public class GetProposalByIdUseCase : IGetProposalByIdPort
{
    private readonly IProposalRepositoryPort _proposalRepository;

    public GetProposalByIdUseCase(IProposalRepositoryPort proposalRepository)
    {
        _proposalRepository = proposalRepository;
    }

    public async Task<ProposalResult?> ExecuteAsync(Guid id)
    {
        var proposal = await _proposalRepository.GetByIdAsync(id);
        
        if (proposal == null)
            return null;

        return new ProposalResult(
            proposal.Id,
            proposal.CustomerName,
            proposal.CustomerEmail,
            proposal.InsuranceType,
            proposal.CoverageAmount,
            proposal.PremiumAmount,
            proposal.Status,
            proposal.CreatedAt,
            proposal.UpdatedAt,
            proposal.RejectionReason
        );
    }
} 