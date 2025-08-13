using Ballware.Meta.Caching;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Caching;

public class CachableTenantRepository<TRepository> 
    : ITenantMetaRepository
    where TRepository : ITenantMetaRepository
{
    private static string CacheKey => "tenant";
    
    private ITenantAwareEntityCache Cache { get; }
    private TRepository Repository { get; }

    public CachableTenantRepository(
        ITenantAwareEntityCache cache,
        TRepository repository)
    {
        Cache = cache;
        Repository = repository;
    }

    public async Task<Public.Tenant?> ByIdAsync(Guid id)
    {
        if (!Cache.TryGetItem(id, CacheKey, id.ToString(), out Public.Tenant? tenant))
        {
            tenant = await Repository.ByIdAsync(id);

            if (tenant != null)
            {
                Cache.SetItem(id, CacheKey, tenant.Id.ToString(), tenant);
            }
        }

        return tenant;
    }

    public async Task<IEnumerable<TenantSelectListEntry>> AllowedTenantsAsync(IDictionary<string, object> claims)
    {
        return await Repository.AllowedTenantsAsync(claims);
    }

    public async Task<IEnumerable<TenantSelectListEntry>> SelectListAsync()
    {
        return await Repository.SelectListAsync();
    }

    public async Task<TenantSelectListEntry?> SelectByIdAsync(Guid id)
    {
        return await Repository.SelectByIdAsync(id);
    }

    public async Task<string> GenerateListQueryAsync()
    {
        return await Repository.GenerateListQueryAsync();
    }

    public async Task<IEnumerable<Tenant>> AllAsync(Guid tenantId, string identifier, IDictionary<string, object> claims)
    {
        return await Repository.AllAsync(tenantId, identifier, claims);
    }

    public async Task<IEnumerable<Tenant>> QueryAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.QueryAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task<long> CountAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.CountAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task<Tenant?> ByIdAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, Guid id)
    {
        return await Repository.ByIdAsync(tenantId, identifier, claims, id);
    }

    public async Task<Tenant> NewAsync(Guid tenantId, string identifier, IDictionary<string, object> claims)
    {
        return await Repository.NewAsync(tenantId, identifier, claims);
    }

    public async Task<Tenant> NewQueryAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.NewQueryAsync(tenantId, identifier, claims, queryParams);
    }

    public async Task SaveAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims,
        Public.Tenant value)
    {
        await Repository.SaveAsync(tenantId, userId, identifier, claims, value);
        
        Cache.PurgeItem(tenantId, CacheKey, value.Id.ToString());
    }

    public async Task<RemoveResult<Public.Tenant>> RemoveAsync(Guid tenantId, Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams)
    {
        var result = await Repository.RemoveAsync(tenantId, userId, claims, removeParams);

        if (result.Value != null)
        {
            Cache.PurgeItem(tenantId, CacheKey, result.Value.Id.ToString());
        }
        
        return result;
    }

    public async Task ImportAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, Stream importStream,
        Func<Tenant, Task<bool>> authorized)
    {
        await Repository.ImportAsync(tenantId, userId, identifier, claims, importStream, authorized);
    }

    public async Task<ExportResult> ExportAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return await Repository.ExportAsync(tenantId, identifier, claims, queryParams);
    }
}