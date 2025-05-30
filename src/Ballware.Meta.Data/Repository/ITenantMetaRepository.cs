using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface ITenantMetaRepository : ITenantableRepository<Public.Tenant>
{
    Task<Public.Tenant?> ByIdAsync(Guid id);
    Task<IEnumerable<TenantSelectListEntry>> AllowedTenantsAsync(IDictionary<string, object> claims);
    
    Task<IEnumerable<TenantSelectListEntry>> SelectListAsync();
    Task<TenantSelectListEntry?> SelectByIdAsync(Guid id);
    
    Task<string> GenerateListQueryAsync();
}