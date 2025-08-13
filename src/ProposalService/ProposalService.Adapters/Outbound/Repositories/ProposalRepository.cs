using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Ports.Outbound;
using ProposalService.Infrastructure.Data;

namespace ProposalService.Adapters.Outbound.Repositories;

public class ProposalRepository : IProposalRepositoryPort
{
    private readonly ProposalDbContext _context;

    public ProposalRepository(ProposalDbContext context)
    {
        _context = context;
    }

    public async Task<Proposal?> GetByIdAsync(Guid id)
    {
        var proposal = await _context.Proposals.FindAsync(id);
        Console.WriteLine($"GetByIdAsync called with id: {id}, found: {proposal != null}");
        return proposal;
    }

    public async Task<IEnumerable<Proposal>> GetAllAsync()
    {
        return await _context.Proposals.ToListAsync();
    }

    public async Task<IEnumerable<Proposal>> GetByStatusAsync(ProposalStatus status)
    {
        return await _context.Proposals
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task<Proposal> AddAsync(Proposal proposal)
    {
        _context.Proposals.Add(proposal);
        await _context.SaveChangesAsync();
        return proposal;
    }

    public async Task<Proposal> UpdateAsync(Proposal proposal)
    {
        _context.Proposals.Update(proposal);
        await _context.SaveChangesAsync();
        return proposal;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Proposals.AnyAsync(p => p.Id == id);
    }
} 