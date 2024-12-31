using Ballware.Meta.Caching;

namespace Ballware.Meta.Data.Ef.Internal;

class CachableTenantMetaRepository : TenantMetaRepository
{
    private ITenantAwareEntityCache Cache { get; }
    
    public CachableTenantMetaRepository(MetaDbContext dbContext, ITenantAwareEntityCache cache)
        : base(dbContext)
    {
        Cache = cache;
    }
    
    public override async Task<Tenant?> ByIdAsync(Guid id)
    {
        if (!Cache.TryGetItem<Tenant>(id, "tenant", id.ToString(), out Tenant? tenant))
        {
            tenant = await base.ByIdAsync(id);

            if (tenant != null)
            {
                Cache.SetItem(id, "tenant", id.ToString(), tenant);    
            }
        }

        return tenant;
    }
}