namespace InsuranceSystem.Shared.Infrastructure.Messaging.Events;

public record ProposalCreated(
    Guid ProposalId,
    string CustomerName,
    string CustomerEmail,
    string InsuranceType,
    decimal CoverageAmount,
    decimal PremiumAmount,
    string Status,
    DateTime CreatedAt
); 