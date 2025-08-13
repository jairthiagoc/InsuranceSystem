namespace InsuranceSystem.Shared.Infrastructure.Messaging.Events;

public record ProposalUpdated(
    Guid ProposalId,
    string CustomerName,
    string CustomerEmail,
    string InsuranceType,
    decimal CoverageAmount,
    decimal PremiumAmount,
    string Status,
    DateTime UpdatedAt
); 