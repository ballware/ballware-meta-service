using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;

namespace Ballware.Meta.Api.Public;

public class EditingEndpointBuilderFactory
{
    private ITenantMetaRepository TenantMetaRepository { get; }
    private IEntityMetaRepository EntityMetaRepository { get; }
    private ITenantRightsChecker TenantRightsChecker { get; }
    private IEntityRightsChecker EntityRightsChecker { get; }
    
    public EditingEndpointBuilderFactory(ITenantMetaRepository tenantMetaRepository,
        IEntityMetaRepository entityMetaRepository,
        ITenantRightsChecker tenantRightsChecker,
        IEntityRightsChecker entityRightsChecker)
    {
        TenantMetaRepository = tenantMetaRepository;
        EntityMetaRepository = entityMetaRepository;
        TenantRightsChecker = tenantRightsChecker;
        EntityRightsChecker = entityRightsChecker;
    }
    
    public EditingEndpointBuilder Create(Guid tenantId, string application, string entity)
    {
        return EditingEndpointBuilder.Create(TenantMetaRepository, EntityMetaRepository, TenantRightsChecker, EntityRightsChecker, tenantId, application, entity);
    }
}