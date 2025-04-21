using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IMlModelMetaRepository : ITenantableRepository<Public.MlModel>
{
    Task<Public.MlModel?> MetadataByTenantAndIdAsync(Public.Tenant tenant, Guid id);
    Task<Public.MlModel?> MetadataByTenantAndIdentifierAsync(Public.Tenant tenant, string identifier);

    Task SaveTrainingStateAsync(Public.Tenant tenant, Guid userId, MlModelTrainingState state);
    
    Task<IEnumerable<MlModelSelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<MlModelSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
    
    Task<string> GenerateListQueryAsync(Guid tenantId);
}