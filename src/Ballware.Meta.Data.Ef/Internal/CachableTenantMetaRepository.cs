using AutoMapper;
using Ballware.Meta.Caching;

namespace Ballware.Meta.Data.Ef.Internal;

class CachableTenantMetaRepository : TenantMetaRepository
{
    private static string CacheKey => "tenant";
    
    private ITenantAwareEntityCache Cache { get; }

    public CachableTenantMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantAwareEntityCache cache)
        : base(mapper, dbContext)
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
}