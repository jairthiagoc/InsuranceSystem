namespace InsuranceSystem.Shared.Infrastructure.Messaging.Events;

public record ProposalStatusUpdated(
    Guid ProposalId,
    string Status,
    string? RejectionReason,
    DateTime UpdatedAt
); 