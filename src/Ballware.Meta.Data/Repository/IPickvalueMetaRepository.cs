using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IPickvalueMetaRepository : ITenantableRepository<Public.Pickvalue>
{
    Task<IEnumerable<PickvalueSelectEntry>> SelectListForEntityFieldAsync(Guid tenantId, string entity, string field);
    Task<PickvalueSelectEntry?> SelectByValueAsync(Guid tenantId, string entity, string field, int value);
    Task<IEnumerable<PickvalueAvailability>> GetPickvalueAvailabilityAsync(Guid tenantId);
    Task<string> GenerateListQueryAsync(Guid tenantId);
    Task<string> GenerateAvailableQueryAsync(Guid tenantId, string entity, string field);
}