using ProposalService.Ports.Inbound.Shared;

namespace ProposalService.Ports.Inbound;

public interface IGetProposalsPort
{
    Task<IEnumerable<ProposalResult>> ExecuteAsync();
} 