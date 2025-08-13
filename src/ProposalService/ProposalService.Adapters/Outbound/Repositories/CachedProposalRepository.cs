using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Ports.Outbound;

namespace ProposalService.Adapters.Outbound.Repositories;

public class CachedProposalRepository : IProposalRepositoryPort
{
    private readonly ProposalRepository _inner;
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public CachedProposalRepository(ProposalRepository inner, IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    private static string ProposalKey(Guid id) => $"proposals:{id}";
    private static string AllProposalsKey => "proposals:all";
    private static DistributedCacheEntryOptions DefaultEntryOptions => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public async Task<Proposal?> GetByIdAsync(Guid id)
    {
        var key = ProposalKey(id);
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
            return JsonSerializer.Deserialize<Proposal>(cached, _jsonOptions);

        var entity = await _inner.GetByIdAsync(id);
        if (entity != null)
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity, _jsonOptions), DefaultEntryOptions);
        return entity;
    }

    public async Task<IEnumerable<Proposal>> GetAllAsync()
    {
        var cached = await _cache.GetStringAsync(AllProposalsKey);
        if (cached != null)
            return JsonSerializer.Deserialize<IEnumerable<Proposal>>(cached, _jsonOptions) ?? Enumerable.Empty<Proposal>();

        var list = (await _inner.GetAllAsync()).ToList();
        await _cache.SetStringAsync(AllProposalsKey, JsonSerializer.Serialize(list, _jsonOptions), DefaultEntryOptions);
        return list;
    }

    public Task<IEnumerable<Proposal>> GetByStatusAsync(ProposalStatus status)
    {
        // Keep simple: delegate; can be cached by specific status if needed later
        return _inner.GetByStatusAsync(status);
    }

    public async Task<Proposal> AddAsync(Proposal proposal)
    {
        var created = await _inner.AddAsync(proposal);
        await _cache.RemoveAsync(AllProposalsKey);
        await _cache.RemoveAsync(ProposalKey(created.Id));
        return created;
    }

    public async Task<Proposal> UpdateAsync(Proposal proposal)
    {
        var updated = await _inner.UpdateAsync(proposal);
        await _cache.RemoveAsync(AllProposalsKey);
        await _cache.RemoveAsync(ProposalKey(updated.Id));
        return updated;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return _inner.ExistsAsync(id);
    }
} 