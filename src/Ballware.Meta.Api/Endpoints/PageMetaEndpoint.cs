using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class PageMetaEndpoint
{
    public static IEndpointRouteBuilder MapPageMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Page",
        string apiOperationPrefix = "Page",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/pagedataforidentifier/{identifier}", HandleMetadataByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Page>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataForPageByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query metadata for page by identifier");
        
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<PageSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all pages");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<PageSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapPageServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Page",
        string apiOperationPrefix = "Page",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        return app;
    }
    
    private static async Task<IResult> HandleMetadataByIdentifierAsync(IPrincipalUtils principalUtils, IPageMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var entry = await repository.ByIdentifierAsync(tenantId, identifier);
        
        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, IPageMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, IPageMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        
        var entry = await repository.SelectByIdForTenantAsync(tenantId, id);

        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
}