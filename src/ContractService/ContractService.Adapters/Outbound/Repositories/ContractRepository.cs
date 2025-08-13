using Microsoft.EntityFrameworkCore;
using ContractService.Domain.Entities;
using ContractService.Ports.Outbound;
using ContractService.Infrastructure.Data;

namespace ContractService.Adapters.Outbound.Repositories;

public class ContractRepository : IContractRepositoryPort
{
    private readonly ContractDbContext _context;

    public ContractRepository(ContractDbContext context)
    {
        _context = context;
    }

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        return await _context.Contracts.FindAsync(id);
    }

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        return await _context.Contracts.ToListAsync();
    }

    public async Task<Contract?> GetByProposalIdAsync(Guid proposalId)
    {
        return await _context.Contracts
            .FirstOrDefaultAsync(c => c.ProposalId == proposalId);
    }

    public async Task<Contract> AddAsync(Contract contract)
    {
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        return contract;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Contracts.AnyAsync(c => c.Id == id);
    }
} 