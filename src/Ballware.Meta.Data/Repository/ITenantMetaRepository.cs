using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface ITenantMetaRepository : IRepository<Public.Tenant>
{
    Task<Public.Tenant?> ByIdAsync(Guid id);
    Task<IEnumerable<TenantSelectListEntry>> AllowedTenantsAsync(Dictionary<string, object> claims);
}