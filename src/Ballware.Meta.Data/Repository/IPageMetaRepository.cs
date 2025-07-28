using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Repository;

public interface IPageMetaRepository : ITenantableRepository<Public.Page>
{
    Task<Public.Page?> ByIdentifierAsync(Guid tenantId, string identifier);
    
    Task<IEnumerable<PageSelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<PageSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
    
    Task<string> GenerateListQueryAsync(Guid tenantId);
}