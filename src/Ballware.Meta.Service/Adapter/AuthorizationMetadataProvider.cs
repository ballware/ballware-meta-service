using Ballware.Meta.Data.Repository;
using Ballware.Shared.Authorization;

namespace Ballware.Meta.Service.Adapter;

public class AuthorizationMetadataProvider(ITenantMetaRepository tenantMetaRepository, IEntityMetaRepository entityMetaRepository) : IAuthorizationMetadataProvider
{
    public async Task<ITenantAuthorizationMetadata?> MetadataForTenantByIdAsync(Guid tenantId)
    {
        return await tenantMetaRepository.ByIdAsync(tenantId);
    }

    public async Task<IEntityAuthorizationMetadata?> MetadataForEntityByTenantAndIdentifierAsync(Guid tenantId, string entity)
    {
        return await entityMetaRepository.ByEntityAsync(tenantId, entity);
    }
}