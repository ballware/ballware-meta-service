using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface ITenantMetaRepository : IRepository<Public.Tenant>
{
    Task<Public.Tenant?> ByIdAsync(Guid id);
    Task<Public.TenantDatabaseObject?> DatabaseObjectByIdAsync(Guid tenant, Guid id);
    Task<IEnumerable<TenantSelectListEntry>> AllowedTenantsAsync(Dictionary<string, object> claims);
    
    Task<IEnumerable<TenantSelectListEntry>> SelectListAsync();
    Task<TenantSelectListEntry?> SelectByIdAsync(Guid id);
    
    Task<string> GenerateListQueryAsync();
}