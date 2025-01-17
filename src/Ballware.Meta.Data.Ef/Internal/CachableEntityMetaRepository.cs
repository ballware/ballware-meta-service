using AutoMapper;
using Ballware.Meta.Caching;
using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Data.Ef.Internal;

class CachableEntityMetaRepository : EntityMetaRepository
{
    private ITenantAwareEntityCache Cache { get; }

    public CachableEntityMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantAwareEntityCache cache)
        : base(mapper, dbContext)
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
                Cache.SetItem(tenantId, "entity", result.Id.ToString(), result);
                Cache.SetItem(tenantId, "entity", result.Entity, result);
            }
        }

        return result;
    }
}