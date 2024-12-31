using Ballware.Meta.Caching;

namespace Ballware.Meta.Data.Ef.Internal;

class CachableLookupMetaRepository : LookupMetaRepository
{
    private ITenantAwareEntityCache Cache { get; }
    
    public CachableLookupMetaRepository(MetaDbContext dbContext, ITenantAwareEntityCache cache)
        : base(dbContext)
    {
        Cache = cache;
    }
    
    public override async Task<Lookup?> ByIdAsync(Guid tenantId, Guid id)
    {
        if (!Cache.TryGetItem(tenantId, "lookup", id.ToString(), out Lookup? result))
        {
            result = await base.ByIdAsync(tenantId, id);
            
            if (result != null && result.Identifier != null)
            {
                Cache.SetItem(tenantId, "lookup", result.Uuid.ToString(), result);
                Cache.SetItem(tenantId, "lookup", result.Identifier, result);
            }
        }

        return result;
    }
    
    public override async Task<Lookup?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        if (!Cache.TryGetItem(tenantId, "lookup", identifier, out Lookup? result))
        {
            result = await base.ByIdentifierAsync(tenantId, identifier);
            
            if (result != null && result.Identifier != null)
            {
                Cache.SetItem(tenantId, "lookup", result.Uuid.ToString(), result);
                Cache.SetItem(tenantId, "lookup", result.Identifier, result);
            }
        }

        return result;
    }
}