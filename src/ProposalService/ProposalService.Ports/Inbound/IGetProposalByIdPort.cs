using ProposalService.Ports.Inbound.Shared;

namespace ProposalService.Ports.Inbound;

public interface IGetProposalByIdPort
{
    Task<ProposalResult?> ExecuteAsync(Guid id);
} 