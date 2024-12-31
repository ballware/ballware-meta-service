namespace Ballware.Meta.Data.Repository;

public interface IMlModelMetaRepository
{
    Task<MlModel?> MetadataByTenantAndIdAsync(Tenant tenant, Guid id);
    Task<MlModel?> MetadataByTenantAndIdentifierAsync(Tenant tenant, string identifier);

    Task SaveTrainingStateAsync(Tenant tenant, Guid userId, MlModelTrainingState state);
}