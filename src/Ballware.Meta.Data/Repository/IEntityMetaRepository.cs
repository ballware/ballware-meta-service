using System.Data;
using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IEntityMetaRepository : ITenantableRepository<Public.EntityMetadata>
{
    Task<Public.EntityMetadata?> ByEntityAsync(Guid tenantId, string entity);
    Task<IEnumerable<EntitySelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<EntitySelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
    Task<EntitySelectListEntry?> SelectByIdentifierForTenantAsync(Guid tenantId, string identifier);
    
    Task<IEnumerable<EntityRightSelectListEntry>> SelectListEntityRightsForTenantAsync(Guid tenantId);
    
    Task<string> GenerateListQueryAsync(Guid tenantId);
    Task<string> GenerateRightsListQueryAsync(Guid tenantId);
}