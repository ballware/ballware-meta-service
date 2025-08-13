using Ballware.Meta.Caching;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Caching;

public class CachableEntityRepository<TRepository> 
    : IEntityMetaRepository
    where TRepository : IEntityMetaRepository
{
    private static string CacheKey => "entity";
    
    private ITenantAwareEntityCache Cache { get; }
    private TRepository Repository { get; }

    public CachableEntityRepository(
        ITenantAwareEntityCache cache, 
        TRepository repository)
    {
        Cache = cache;
        Repository = repository;
    }
    
    public async Task<EntityMetadata?> ByEntityAsync(Guid tenantId, string entity)
    {
        if (!Cache.TryGetItem(tenantId, CacheKey, entity, out EntityMetadata? result))
        {
            result = await Repository.ByEntityAsync(tenantId, entity);

            if (result != null && result.Entity != null)
            {
                Cache.SetItem(tenantId, CacheKey, result.Id.ToString(), result);
                Cache.SetItem(tenantId, CacheKey, result.Entity, result);
            }
        }

        return result;
    }
    
    public async Task SaveAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims,
        EntityMetadata value)
    {
        await Repository.SaveAsync(tenantId, userId, identifier, claims, value);
        
        Cache.PurgeItem(tenantId, CacheKey, value.Id.ToString());
        Cache.PurgeItem(tenantId, CacheKey, value.Entity);
    }

    public async Task<RemoveResult<EntityMetadata>> RemoveAsync(Guid tenantId, Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams)
    {
        var result = await Repository.RemoveAsync(tenantId, userId, claims, removeParams);

        if (result.Value != null)
        {
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Id.ToString());
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Entity);
        }
        
        return result;
    }
    
    public async Task<IEnumerable<EntityMetadata>> AllAsync(Guid tenantId, string identifier, IDictionary<string, object> claims)
    {
        return await Repository.AllAsync(tenantId, identifier, claims);
    }

    public async Task<IEnumerable<EntityMetadata>> QueryAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.QueryAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task<long> CountAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.CountAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task<EntityMetadata?> ByIdAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, Guid id)
    {
        return await Repository.ByIdAsync(tenantId, identifier, claims, id);
    }

    public async Task<EntityMetadata> NewAsync(Guid tenantId, string identifier, IDictionary<string, object> claims)
    {
        return await Repository.NewAsync(tenantId, identifier, claims);
    }

    public async Task<EntityMetadata> NewQueryAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.NewQueryAsync(tenantId, identifier, claims, queryParams);
    }
    
    public async Task ImportAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, Stream importStream,
        Func<EntityMetadata, Task<bool>> authorized)
    {
        await Repository.ImportAsync(tenantId, userId, identifier, claims, importStream, authorized);
    }

    public async Task<ExportResult> ExportAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.ExportAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task<IEnumerable<EntitySelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Repository.SelectListForTenantAsync(tenantId);
    }

    public async Task<EntitySelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Repository.SelectByIdForTenantAsync(tenantId, id);
    }

    public async Task<EntitySelectListEntry?> SelectByIdentifierForTenantAsync(Guid tenantId, string identifier)
    {
        return await Repository.SelectByIdentifierForTenantAsync(tenantId, identifier);
    }

    public async Task<IEnumerable<EntityRightSelectListEntry>> SelectListEntityRightsForTenantAsync(Guid tenantId)
    {
        return await Repository.SelectListEntityRightsForTenantAsync(tenantId);
    }

    public async Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return await Repository.GenerateListQueryAsync(tenantId);
    }

    public async Task<string> GenerateRightsListQueryAsync(Guid tenantId)
    {
        return await Repository.GenerateRightsListQueryAsync(tenantId);
    }
}