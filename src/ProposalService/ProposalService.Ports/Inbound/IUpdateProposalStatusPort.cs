using ProposalService.Ports.Inbound.Shared;
using ProposalService.Domain.Enums;

namespace ProposalService.Ports.Inbound;

public interface IUpdateProposalStatusPort
{
    Task<ProposalResult> ExecuteAsync(UpdateProposalStatusRequest request);
}

public record UpdateProposalStatusRequest(
    Guid Id,
    ProposalStatus Status,
    string? RejectionReason = null
); 