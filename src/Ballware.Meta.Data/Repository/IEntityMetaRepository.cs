using System.Data;
using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IEntityMetaRepository
{
    Task<EntityMetadata?> ByEntityAsync(Guid tenantId, string entity);
    Task<IEnumerable<EntityRightSelectListEntry>> SelectListEntityRightsAsync(Guid tenantId);
    Task<IEnumerable<CharacteristicAssociation>> CharacteristicAssociationsAsync(IDbConnection connection, Guid tenantId, Guid id);
    EntityMetadata? ByEntity(IDbConnection connection, Guid tenantId, string entity);
    EntityMetadata? ById(IDbConnection connection, Guid tenantId, Guid id);
    IEnumerable<CharacteristicAssociation> CharacteristicAssociations(IDbConnection connection, Guid tenantId, Guid id);
}