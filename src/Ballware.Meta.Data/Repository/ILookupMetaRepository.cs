using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Repository;

public interface ILookupMetaRepository : ITenantableRepository<Public.Lookup>
{
    Task<IEnumerable<Public.Lookup>> AllForTenantAsync(Guid tenantId);

    Task<Public.Lookup?> ByIdAsync(Guid tenantId, Guid id);

    Task<Public.Lookup?> ByIdentifierAsync(Guid tenantId, string identifier);
    
    Task<IEnumerable<LookupSelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<LookupSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
    
    Task<string> GenerateListQueryAsync(Guid tenantId);
}