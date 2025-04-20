using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class TenantMetaEndpoint
{
    private static readonly string DefaultQuery = "primary";

    public static IEndpointRouteBuilder MapTenantMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Tenant",
        string apiOperationPrefix = "Tenant",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/metadatafortenant/{tenantId}", HandleMetadataForTenantAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Tenant>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "Metadata")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query tenant metadata by id");
        
        app.MapGet(basePath + "/allowed", HandleAllowedTenantsForUserAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<TenantSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "Allowed")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query allowed tenants for user");

        return app;
    }

    public static IEndpointRouteBuilder MapTenantServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Tenant",
        string apiOperationPrefix = "Tenant",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {
        return app;
    }

    public static async Task<IResult> HandleMetadataForTenantAsync(IPrincipalUtils principalUtils, ClaimsPrincipal user,
        ITenantMetaRepository tenantMetaRepository, Guid tenantId)
    {
        var userTenantId = principalUtils.GetUserTenandId(user);

        if (userTenantId != tenantId)
            return Results.Forbid();

        return Results.Ok(await tenantMetaRepository.ByIdAsync(tenantId));
    }
    
    public static async Task<IResult> HandleAllowedTenantsForUserAsync(IPrincipalUtils principalUtils, ClaimsPrincipal user,
        ITenantMetaRepository tenantMetaRepository)
    {
        var claims = principalUtils.GetUserClaims(user);

        return Results.Ok(await tenantMetaRepository.AllowedTenantsAsync(claims));
    }
}