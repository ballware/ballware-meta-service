using AutoMapper;
using Ballware.Meta.Caching;

namespace Ballware.Meta.Data.Ef.Internal;

class CachableLookupMetaRepository : LookupMetaRepository
{
    private ITenantAwareEntityCache Cache { get; }
    
    public CachableLookupMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantAwareEntityCache cache)
        : base(mapper, dbContext)
    {
        Cache = cache;
    }
    
    public override async Task<Public.Lookup?> ByIdAsync(Guid tenantId, Guid id)
    {
        if (!Cache.TryGetItem(tenantId, "lookup", id.ToString(), out Public.Lookup? result))
        {
            result = await base.ByIdAsync(tenantId, id);
            
            if (result != null && result.Identifier != null)
            {
                Cache.SetItem(tenantId, "lookup", result.Id.ToString(), result);
                Cache.SetItem(tenantId, "lookup", result.Identifier, result);
            }
        }

        return result;
    }
    
    public override async Task<Public.Lookup?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        if (!Cache.TryGetItem(tenantId, "lookup", identifier, out Public.Lookup? result))
        {
            result = await base.ByIdentifierAsync(tenantId, identifier);
            
            if (result != null && result.Identifier != null)
            {
                Cache.SetItem(tenantId, "lookup", result.Id.ToString(), result);
                Cache.SetItem(tenantId, "lookup", result.Identifier, result);
            }
        }

        return result;
    }
}