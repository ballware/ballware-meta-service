using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Repository;

public interface IMlModelMetaRepository : ITenantableRepository<Public.MlModel>
{
    Task<Public.MlModel?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id);
    Task<Public.MlModel?> MetadataByTenantAndIdentifierAsync(Guid tenantId, string identifier);

    Task SaveTrainingStateAsync(Guid tenantId, Guid userId, MlModelTrainingState state);
    
    Task<IEnumerable<MlModelSelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<MlModelSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
    
    Task<string> GenerateListQueryAsync(Guid tenantId);
}