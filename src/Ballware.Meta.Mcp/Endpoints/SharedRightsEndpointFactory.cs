using System.Collections.Immutable;
using System.Security.Claims;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Mcp.Endpoints;

public static class SharedRightsEndpointFactory
{
    public static Func<IServiceProvider, ClaimsPrincipal, Task<bool>>? CreateStaticEntityRightAuthorizationHandler(string application, string entity, string right) 
    {
        return async (serviceProvider, principal) =>
        {
            var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
            var tenantId = principalUtils.GetUserTenandId(principal);
            var tenantRepository = serviceProvider.GetRequiredService<ITenantMetaRepository>();
            var entityRepository = serviceProvider.GetRequiredService<IEntityMetaRepository>();
            var tenantRightsChecker = serviceProvider.GetRequiredService<ITenantRightsChecker>();
            var entityRightsChecker = serviceProvider.GetRequiredService<IEntityRightsChecker>();

            var tenantMetadata = await tenantRepository.ByIdAsync(tenantId);
            var entityMetadata = await entityRepository.ByEntityAsync(tenantId, entity);
            var claims = principalUtils.GetUserClaims(principal);

            if (tenantMetadata == null)
            {
                return false;
            }

            if (entityMetadata == null)
            {
                return false;
            }

            var tenantAllowed = await tenantRightsChecker.HasRightAsync(tenantMetadata, application, entity, claims, right);
        
            return await entityRightsChecker.HasRightAsync(tenantId, entityMetadata, claims, right, null, tenantAllowed);    
        };
    }
}