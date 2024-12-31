using System.Data;
using System.Data.Common;
using Ballware.Meta.Caching;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class CachableEntityMetaRepository : EntityMetaRepository
{
    private ITenantAwareEntityCache Cache { get; }
    
    public CachableEntityMetaRepository(MetaDbContext dbContext, ITenantAwareEntityCache cache) 
        : base(dbContext)
    {
        Cache = cache;
    }
    
    public override async Task<EntityMetadata?> ByEntityAsync(Guid tenantId, string entity)
    {
        if (!Cache.TryGetItem(tenantId, "entity", entity, out EntityMetadata? result))
        {
            result = await base.ByEntityAsync(tenantId, entity);

            if (result != null && result.Entity != null)
            {
                Cache.SetItem(tenantId, "entity", result.Uuid.ToString(), result);
                Cache.SetItem(tenantId, "entity", result.Entity, result);    
            }
        }

        return result;
    }
}