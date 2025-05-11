using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IEntityRightMetaRepository : ITenantableRepository<Public.EntityRight>
{
    Task<IEnumerable<EntityRightSelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<EntityRightSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
}