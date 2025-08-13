namespace ContractService.Ports.Inbound.Shared;

public record ContractResult(
    Guid Id,
    Guid ProposalId,
    DateTime ContractDate,
    string ContractNumber,
    decimal PremiumAmount,
    DateTime CreatedAt
); 