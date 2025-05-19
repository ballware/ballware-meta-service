using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class LookupMetaEndpoint
{
    public static IEndpointRouteBuilder MapLookupMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Lookup",
        string apiOperationPrefix = "Lookup",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<LookupSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all lookup");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<LookupSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapLookupServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Lookup",
        string apiOperationPrefix = "Lookup",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/lookupmetadatabytenantandid/{tenantId}/{id}", HandleMetadataForTenantAndIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Lookup>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataForTenantAndId")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query lookup metadata by tenant and identifier");
        
        app.MapGet(basePath + "/lookupmetadatabytenantandidentifier/{tenantId}/{identifier}", HandleMetadataForTenantAndIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Lookup>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataForTenantAndIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query lookup metadata by tenant and identifier");
        
        app.MapGet(basePath + "/lookupmetadatabytenant/{tenantId}", HandleMetadataForTenantAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<Lookup>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataForTenant")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query lookup metadata by tenant");
        
        return app;
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, ILookupMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, ILookupMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectByIdForTenantAsync(tenantId, id));
    }
    
    private static async Task<IResult> HandleMetadataForTenantAndIdAsync(ILookupMetaRepository repository, Guid tenantId, Guid id)
    {
        var lookup = await repository.ByIdAsync(tenantId, id);

        return Results.Ok(lookup);
    }
    
    private static async Task<IResult> HandleMetadataForTenantAndIdentifierAsync(ILookupMetaRepository repository, Guid tenantId, string identifier)
    {
        var lookup = await repository.ByIdentifierAsync(tenantId, identifier);

        return Results.Ok(lookup);
    }
    
    private static async Task<IResult> HandleMetadataForTenantAsync(ILookupMetaRepository repository, Guid tenantId)
    {
        var lookups = await repository.AllForTenantAsync(tenantId);

        return Results.Ok(lookups);
    }
}