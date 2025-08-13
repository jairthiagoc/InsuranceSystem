namespace ContractService.Ports.Outbound;

public interface IProposalServicePort
{
    Task<ProposalInfo?> GetProposalAsync(Guid proposalId);
}

public record ProposalInfo(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    string InsuranceType,
    decimal CoverageAmount,
    decimal PremiumAmount,
    string Status
); 