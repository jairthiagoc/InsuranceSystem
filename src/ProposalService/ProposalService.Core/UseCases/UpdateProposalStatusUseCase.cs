using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Ports.Outbound;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;

namespace ProposalService.Core.UseCases;

public class UpdateProposalStatusUseCase : IUpdateProposalStatusPort
{
    private readonly IProposalRepositoryPort _proposalRepository;
    private readonly IEventPublisherPort _eventPublisher;

    public UpdateProposalStatusUseCase(IProposalRepositoryPort proposalRepository, IEventPublisherPort eventPublisher)
    {
        _proposalRepository = proposalRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<ProposalResult> ExecuteAsync(UpdateProposalStatusRequest request)
    {
        var proposal = await _proposalRepository.GetByIdAsync(request.Id);
        
        if (proposal == null)
            throw new ArgumentException($"Proposal with ID {request.Id} not found");

        proposal.UpdateStatus(request.Status, request.RejectionReason);
        
        var updatedProposal = await _proposalRepository.UpdateAsync(proposal);

        await _eventPublisher.PublishAsync(new ProposalStatusUpdated(
            updatedProposal.Id,
            updatedProposal.Status.ToString(),
            updatedProposal.RejectionReason,
            updatedProposal.UpdatedAt
        ));

        return new ProposalResult(
            updatedProposal.Id,
            updatedProposal.CustomerName,
            updatedProposal.CustomerEmail,
            updatedProposal.InsuranceType,
            updatedProposal.CoverageAmount,
            updatedProposal.PremiumAmount,
            updatedProposal.Status,
            updatedProposal.CreatedAt,
            updatedProposal.UpdatedAt,
            updatedProposal.RejectionReason
        );
    }
} 