using ProposalService.Ports.Inbound.Shared;

namespace ProposalService.Ports.Inbound;

public interface ICreateProposalPort
{
    Task<ProposalResult> ExecuteAsync(CreateProposalRequest request);
}

public record CreateProposalRequest(
    string CustomerName,
    string CustomerEmail,
    string InsuranceType,
    decimal CoverageAmount,
    decimal PremiumAmount
); 