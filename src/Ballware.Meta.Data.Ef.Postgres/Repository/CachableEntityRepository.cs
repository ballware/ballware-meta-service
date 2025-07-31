using AutoMapper;
using Ballware.Meta.Caching;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

public class CachableEntityRepository : EntityRepository
{
    private static string CacheKey => "entity";
    
    private ITenantAwareEntityCache Cache { get; }

    public CachableEntityRepository(IMapper mapper, IMetaDbContext dbContext,
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
        if (!Cache.TryGetItem(tenantId, CacheKey, entity, out EntityMetadata? result))
        {
            result = await base.ByEntityAsync(tenantId, entity);

            if (result != null && result.Entity != null)
            {
                Cache.SetItem(tenantId, CacheKey, result.Id.ToString(), result);
                Cache.SetItem(tenantId, CacheKey, result.Entity, result);
            }
        }

        return result;
    }
    
    public override async Task SaveAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims,
        EntityMetadata value)
    {
        await base.SaveAsync(tenantId, userId, identifier, claims, value);
        
        Cache.PurgeItem(tenantId, CacheKey, value.Id.ToString());
        Cache.PurgeItem(tenantId, CacheKey, value.Entity);
    }

    public override async Task<RemoveResult<EntityMetadata>> RemoveAsync(Guid tenantId, Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams)
    {
        var result = await base.RemoveAsync(tenantId, userId, claims, removeParams);

        if (result.Value != null)
        {
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Id.ToString());
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Entity);
        }
        
        return result;
    }
}
