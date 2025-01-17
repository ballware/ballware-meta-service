using Ballware.Meta.Data.Common;

namespace Ballware.Meta.Data.Repository;

public interface IMlModelMetaRepository : ITenantableRepository<Public.MlModel>
{
    Task<Public.MlModel?> MetadataByTenantAndIdAsync(Public.Tenant tenant, Guid id);
    Task<Public.MlModel?> MetadataByTenantAndIdentifierAsync(Public.Tenant tenant, string identifier);

    Task SaveTrainingStateAsync(Public.Tenant tenant, Guid userId, MlModelTrainingState state);
}