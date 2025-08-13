using ProposalService.Ports.Inbound.Shared;
using ProposalService.Domain.Enums;

namespace ProposalService.Ports.Inbound;

public interface IGetProposalsByStatusPort
{
    Task<IEnumerable<ProposalResult>> ExecuteAsync(ProposalStatus status);
} 