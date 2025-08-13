using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;

namespace ProposalService.Ports.Outbound;

public interface IProposalRepositoryPort
{
    Task<Proposal?> GetByIdAsync(Guid id);
    Task<IEnumerable<Proposal>> GetAllAsync();
    Task<IEnumerable<Proposal>> GetByStatusAsync(ProposalStatus status);
    Task<Proposal> AddAsync(Proposal proposal);
    Task<Proposal> UpdateAsync(Proposal proposal);
    Task<bool> ExistsAsync(Guid id);
} 