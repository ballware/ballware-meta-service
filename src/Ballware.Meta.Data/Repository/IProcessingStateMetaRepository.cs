using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IProcessingStateMetaRepository : ITenantableRepository<Public.ProcessingState>
{
    Task<IEnumerable<ProcessingStateSelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<ProcessingStateSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
    
    Task<IEnumerable<ProcessingStateSelectListEntry>> SelectListForEntityAsync(Guid tenantId, string entity);
    Task<ProcessingStateSelectListEntry?> SelectByStateAsync(Guid tenantId, string entity, int state);
    Task<IEnumerable<ProcessingStateSelectListEntry>> SelectListPossibleSuccessorsForEntityAsync(Guid tenantId,
        string entity, int state);

    Task<string> GenerateListQueryAsync(Guid tenantId);
}