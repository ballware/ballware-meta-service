using System.Data;
using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IEntityMetaRepository : ITenantableRepository<Public.EntityMetadata>
{
    Task<Public.EntityMetadata?> ByEntityAsync(Guid tenantId, string entity);
    Task<IEnumerable<Public.CharacteristicAssociation>> CharacteristicAssociationsAsync(IDbConnection connection, Guid tenantId, Guid id);
    Public.EntityMetadata? ByEntity(IDbConnection connection, Guid tenantId, string entity);
    Public.EntityMetadata? ById(IDbConnection connection, Guid tenantId, Guid id);
    IEnumerable<Public.CharacteristicAssociation> CharacteristicAssociations(IDbConnection connection, Guid tenantId, Guid id);
    
    Task<IEnumerable<EntitySelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<EntitySelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
    Task<EntitySelectListEntry?> SelectByIdentifierForTenantAsync(Guid tenantId, string identifier);
    
    Task<IEnumerable<EntityRightSelectListEntry>> SelectListEntityRightsForTenantAsync(Guid tenantId);
    Task<EntityRightSelectListEntry?> SelectEntityRightByIdentifierForTenantAsync(Guid tenantId, string identifier);
}