using ProposalService.Domain.Enums;

namespace ProposalService.Ports.Inbound.Shared;

public record ProposalResult(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    string InsuranceType,
    decimal CoverageAmount,
    decimal PremiumAmount,
    ProposalStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? RejectionReason = null
); 