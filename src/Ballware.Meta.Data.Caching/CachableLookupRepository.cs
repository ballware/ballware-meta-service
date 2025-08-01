using Ballware.Meta.Caching;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Caching;

public class CachableLookupRepository<TRepository> 
    : ILookupMetaRepository
    where TRepository : ILookupMetaRepository
{
    private static string CacheKey => "lookup";

    private ITenantAwareEntityCache Cache { get; }
    private TRepository Repository { get; }

    public CachableLookupRepository(ITenantAwareEntityCache cache, 
        TRepository repository)
    {
        Cache = cache;
        Repository = repository;
    }

    public async Task<IEnumerable<Lookup>> AllForTenantAsync(Guid tenantId)
    {
        return await Repository.AllForTenantAsync(tenantId);
    }

    public async Task<Public.Lookup?> ByIdAsync(Guid tenantId, Guid id)
    {
        if (!Cache.TryGetItem(tenantId, CacheKey, id.ToString(), out Public.Lookup? result))
        {
            result = await Repository.ByIdAsync(tenantId, id);

            if (result != null && result.Identifier != null)
            {
                Cache.SetItem(tenantId, CacheKey, result.Id.ToString(), result);
                Cache.SetItem(tenantId, CacheKey, result.Identifier, result);
            }
        }

        return result;
    }

    public async Task<Public.Lookup?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        if (!Cache.TryGetItem(tenantId, CacheKey, identifier, out Public.Lookup? result))
        {
            result = await Repository.ByIdentifierAsync(tenantId, identifier);

            if (result != null && result.Identifier != null)
            {
                Cache.SetItem(tenantId, CacheKey, result.Id.ToString(), result);
                Cache.SetItem(tenantId, CacheKey, result.Identifier, result);
            }
        }

        return result;
    }

    public async Task<IEnumerable<LookupSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Repository.SelectListForTenantAsync(tenantId);
    }

    public async Task<LookupSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Repository.SelectByIdForTenantAsync(tenantId, id);
    }

    public async Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return await Repository.GenerateListQueryAsync(tenantId);
    }

    public async Task<IEnumerable<Lookup>> AllAsync(Guid tenantId, string identifier, IDictionary<string, object> claims)
    {
        return await Repository.AllAsync(tenantId, identifier, claims);
    }

    public async Task<IEnumerable<Lookup>> QueryAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.QueryAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task<long> CountAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.CountAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task<Lookup?> ByIdAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, Guid id)
    {
        return await Repository.ByIdAsync(tenantId, identifier, claims, id);
    }

    public async Task<Lookup> NewAsync(Guid tenantId, string identifier, IDictionary<string, object> claims)
    {
        return await Repository.NewAsync(tenantId, identifier, claims);
    }

    public async Task<Lookup> NewQueryAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.NewQueryAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task SaveAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims,
        Public.Lookup value)
    {
        await Repository.SaveAsync(tenantId, userId, identifier, claims, value);
        
        Cache.PurgeItem(tenantId, CacheKey, value.Id.ToString());
        Cache.PurgeItem(tenantId, CacheKey, value.Identifier);
    }

    public async Task<RemoveResult<Public.Lookup>> RemoveAsync(Guid tenantId, Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams)
    {
        var result = await Repository.RemoveAsync(tenantId, userId, claims, removeParams);

        if (result.Value != null)
        {
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Id.ToString());
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Identifier);
        }
        
        return result;
    }

    public async Task ImportAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, Stream importStream,
        Func<Lookup, Task<bool>> authorized)
    {
        await Repository.ImportAsync(tenantId, userId, identifier, claims, importStream, authorized);
    }

    public async Task<ExportResult> ExportAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.ExportAsync(tenantId, identifier, claims, queryParams);
    }
}