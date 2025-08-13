using ContractService.Domain.Entities;

namespace ContractService.Ports.Outbound;

public interface IContractRepositoryPort
{
    Task<Contract?> GetByIdAsync(Guid id);
    Task<IEnumerable<Contract>> GetAllAsync();
    Task<Contract?> GetByProposalIdAsync(Guid proposalId);
    Task<Contract> AddAsync(Contract contract);
    Task<bool> ExistsAsync(Guid id);
} 