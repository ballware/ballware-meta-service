using AutoMapper;
using Ballware.Meta.Caching;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;

namespace Ballware.Meta.Data.Ef.Internal;

class CachableEntityMetaRepository : EntityMetaRepository
{
    private ITenantAwareEntityCache Cache { get; }

    public CachableEntityMetaRepository(IMapper mapper, MetaDbContext dbContext,
        IProcessingStateMetaRepository processingStateMetaRepository,
        IPickvalueMetaRepository pickvalueMetaRepository,
        IEntityRightMetaRepository entityRightMetaRepository,
        ICharacteristicAssociationMetaRepository characteristicAssociationMetaRepository,
        ITenantAwareEntityCache cache, 
        ITenantableRepositoryHook<Public.EntityMetadata, Persistables.EntityMetadata>? hook = null)
        : base(mapper, processingStateMetaRepository, pickvalueMetaRepository, entityRightMetaRepository, characteristicAssociationMetaRepository, dbContext, hook)
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