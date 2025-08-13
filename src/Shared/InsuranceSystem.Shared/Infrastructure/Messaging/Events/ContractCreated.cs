namespace InsuranceSystem.Shared.Infrastructure.Messaging.Events;

public record ContractCreated(
    Guid ContractId,
    Guid ProposalId,
    string ContractNumber,
    decimal PremiumAmount,
    DateTime ContractDate,
    DateTime CreatedAt
); 