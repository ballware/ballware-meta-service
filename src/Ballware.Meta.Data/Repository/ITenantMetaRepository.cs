using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface ITenantMetaRepository
{
    Task<Tenant?> ByIdAsync(Guid id);
    Task<IEnumerable<TenantSelectListEntry>> AllowedTenantsAsync(Dictionary<string, object> claims);
}