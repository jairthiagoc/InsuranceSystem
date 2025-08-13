using ProposalService.Domain.Entities;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Ports.Outbound;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;

namespace ProposalService.Core.UseCases;

public class CreateProposalUseCase : ICreateProposalPort
{
    private readonly IProposalRepositoryPort _proposalRepository;
    private readonly IEventPublisherPort _eventPublisher;

    public CreateProposalUseCase(IProposalRepositoryPort proposalRepository, IEventPublisherPort eventPublisher)
    {
        _proposalRepository = proposalRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<ProposalResult> ExecuteAsync(CreateProposalRequest request)
    {
        var proposal = new Proposal(
            request.CustomerName,
            request.CustomerEmail,
            request.InsuranceType,
            request.CoverageAmount,
            request.PremiumAmount
        );

        var createdProposal = await _proposalRepository.AddAsync(proposal);

        await _eventPublisher.PublishAsync(new ProposalCreated(
            createdProposal.Id,
            createdProposal.CustomerName,
            createdProposal.CustomerEmail,
            createdProposal.InsuranceType,
            createdProposal.CoverageAmount,
            createdProposal.PremiumAmount,
            createdProposal.Status.ToString(),
            createdProposal.CreatedAt
        ));

        return new ProposalResult(
            createdProposal.Id,
            createdProposal.CustomerName,
            createdProposal.CustomerEmail,
            createdProposal.InsuranceType,
            createdProposal.CoverageAmount,
            createdProposal.PremiumAmount,
            createdProposal.Status,
            createdProposal.CreatedAt,
            createdProposal.UpdatedAt,
            createdProposal.RejectionReason
        );
    }
} 