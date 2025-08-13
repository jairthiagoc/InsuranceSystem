using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ContractService.Domain.Entities;
using ContractService.Ports.Outbound;
using ContractService.Adapters.Outbound.Repositories;

namespace ContractService.Adapters.Outbound.Repositories;

public class CachedContractRepository : IContractRepositoryPort
{
    private readonly ContractRepository _inner;
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public CachedContractRepository(ContractRepository inner, IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    private static string ContractKey(Guid id) => $"contracts:{id}";
    private static string AllContractsKey => "contracts:all";
    private static DistributedCacheEntryOptions DefaultEntryOptions => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        var cacheKey = ContractKey(id);
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<Contract>(cached, _jsonOptions);

        var entity = await _inner.GetByIdAsync(id);
        if (entity != null)
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entity, _jsonOptions), DefaultEntryOptions);
        return entity;
    }

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        var cached = await _cache.GetStringAsync(AllContractsKey);
        if (cached != null)
            return JsonSerializer.Deserialize<IEnumerable<Contract>>(cached, _jsonOptions) ?? Enumerable.Empty<Contract>();

        var list = (await _inner.GetAllAsync()).ToList();
        await _cache.SetStringAsync(AllContractsKey, JsonSerializer.Serialize(list, _jsonOptions), DefaultEntryOptions);
        return list;
    }

    public Task<Contract?> GetByProposalIdAsync(Guid proposalId)
    {
        // Rarely cached; delegate to inner or you can introduce specific key if hotspot
        return _inner.GetByProposalIdAsync(proposalId);
    }

    public async Task<Contract> AddAsync(Contract contract)
    {
        var created = await _inner.AddAsync(contract);
        await _cache.RemoveAsync(AllContractsKey);
        await _cache.RemoveAsync(ContractKey(created.Id));
        return created;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        // Optimization possible using key existence; rely on inner for consistency
        return _inner.ExistsAsync(id);
    }
} 