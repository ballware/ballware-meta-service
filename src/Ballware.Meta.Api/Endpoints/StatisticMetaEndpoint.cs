using System.Security.Claims;
using Ballware.Shared.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class StatisticMetaEndpoint
{
    private const string ApiTag = "Statistic";
    private const string ApiOperationPrefix = "Statistic";
    
    public static IEndpointRouteBuilder MapStatisticMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/metadataforidentifier/{identifier}", HandleMetadataByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Statistic>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "MetadataByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query metadata for statistic by identifier");
        
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<StatisticSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all pages");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<StatisticSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapStatisticServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/metadatafortenantandidentifier/{tenantId}/{identifier}", HandleMetadataByTenantAndIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Statistic>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "MetadataByTenantAndIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query metadata for statistic by tenant and identifier");
        
        return app;
    }
    
    private static async Task<IResult> HandleMetadataByIdentifierAsync(IPrincipalUtils principalUtils, IStatisticMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        
        var entry = await repository.MetadataByIdentifierAsync(tenantId, identifier);
        
        if (entry == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, IStatisticMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, IStatisticMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var entry = await repository.SelectByIdForTenantAsync(tenantId, id);
        
        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleMetadataByTenantAndIdentifierAsync(IStatisticMetaRepository repository, Guid tenantId, string identifier)
    {
        var entry = await repository.MetadataByIdentifierAsync(tenantId, identifier);
        
        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
}