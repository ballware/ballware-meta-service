using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IPickvalueMetaRepository
{
    Task<IEnumerable<PickvalueSelectEntry>> SelectListForEntityFieldAsync(Guid tenantId, string entity, string field);
    Task<PickvalueSelectEntry?> SelectByValueAsync(Guid tenantId, string entity, string field, int value);
    Task<string> GenerateListQueryAsync(Guid tenantId);
}