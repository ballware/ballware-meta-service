using AutoMapper;
using Ballware.Meta.Caching;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

class CachableLookupRepository : LookupRepository
{
    private static string CacheKey => "lookup";

    private ITenantAwareEntityCache Cache { get; }

    public CachableLookupRepository(IMapper mapper, IMetaDbContext dbContext, ITenantAwareEntityCache cache, 
        ITenantableRepositoryHook<Public.Lookup, Persistables.Lookup>? hook = null)
        : base(mapper, dbContext, hook)
    {
        Cache = cache;
    }

    public override async Task<Public.Lookup?> ByIdAsync(Guid tenantId, Guid id)
    {
        if (!Cache.TryGetItem(tenantId, CacheKey, id.ToString(), out Public.Lookup? result))
        {
            result = await base.ByIdAsync(tenantId, id);

            if (result != null && result.Identifier != null)
            {
                Cache.SetItem(tenantId, CacheKey, result.Id.ToString(), result);
                Cache.SetItem(tenantId, CacheKey, result.Identifier, result);
            }
        }

        return result;
    }

    public override async Task<Public.Lookup?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        if (!Cache.TryGetItem(tenantId, CacheKey, identifier, out Public.Lookup? result))
        {
            result = await base.ByIdentifierAsync(tenantId, identifier);

            if (result != null && result.Identifier != null)
            {
                Cache.SetItem(tenantId, CacheKey, result.Id.ToString(), result);
                Cache.SetItem(tenantId, CacheKey, result.Identifier, result);
            }
        }

        return result;
    }
    
    public override async Task SaveAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims,
        Public.Lookup value)
    {
        await base.SaveAsync(tenantId, userId, identifier, claims, value);
        
        Cache.PurgeItem(tenantId, CacheKey, value.Id.ToString());
        Cache.PurgeItem(tenantId, CacheKey, value.Identifier);
    }

    public override async Task<RemoveResult<Public.Lookup>> RemoveAsync(Guid tenantId, Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams)
    {
        var result = await base.RemoveAsync(tenantId, userId, claims, removeParams);

        if (result.Value != null)
        {
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Id.ToString());
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Identifier);
        }
        
        return result;
    }
}
