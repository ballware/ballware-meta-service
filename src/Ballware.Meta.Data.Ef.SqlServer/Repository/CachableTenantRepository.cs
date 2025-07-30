using AutoMapper;
using Ballware.Meta.Caching;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

class CachableTenantRepository : TenantRepository
{
    private static string CacheKey => "tenant";
    
    private ITenantAwareEntityCache Cache { get; }

    public CachableTenantRepository(
        IMapper mapper, MetaDbContext dbContext, 
        ITenantAwareEntityCache cache,
        IRepositoryHook<Public.Tenant, Persistables.Tenant>? hook = null)
        : base(mapper, dbContext, hook)
    {
        Cache = cache;
    }

    public override async Task<Public.Tenant?> ByIdAsync(Guid id)
    {
        if (!Cache.TryGetItem(id, CacheKey, id.ToString(), out Public.Tenant? tenant))
        {
            tenant = await base.ByIdAsync(id);

            if (tenant != null)
            {
                Cache.SetItem(id, CacheKey, tenant.Id.ToString(), tenant);
            }
        }

        return tenant;
    }
    
    public override async Task SaveAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims,
        Public.Tenant value)
    {
        await base.SaveAsync(userId, identifier, claims, value);
        
        Cache.PurgeItem(tenantId, CacheKey, value.Id.ToString());
    }

    public override async Task<RemoveResult<Public.Tenant>> RemoveAsync(Guid tenantId, Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams)
    {
        var result = await base.RemoveAsync(userId, claims, removeParams);

        if (result.Value != null)
        {
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Id.ToString());
        }
        
        return result;
    }
}