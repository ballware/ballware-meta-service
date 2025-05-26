using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;

namespace Ballware.Meta.Api.Public;

public class EditingEndpointBuilderFactory
{
    private ITenantMetaRepository TenantMetaRepository { get; }
    private ITenantRightsChecker TenantRightsChecker { get; }
    
    public EditingEndpointBuilderFactory(ITenantMetaRepository tenantMetaRepository,
        ITenantRightsChecker tenantRightsChecker)
    {
        TenantMetaRepository = tenantMetaRepository;
        TenantRightsChecker = tenantRightsChecker;
    }
    
    public EditingEndpointBuilder Create(string application, string entity)
    {
        return EditingEndpointBuilder.Create(TenantMetaRepository, TenantRightsChecker, application, entity);
    }
}