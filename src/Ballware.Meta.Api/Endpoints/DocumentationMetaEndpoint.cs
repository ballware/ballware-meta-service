using System;
using System.Security.Claims;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class DocumentationMetaEndpoint
{
    public static IEndpointRouteBuilder MapDocumentationMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Documentation",
        string apiOperationPrefix = "Documentation",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/documentationforentity/{entity}", HandleForEntityAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<string>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "ForEntity")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query documentation for entity");
        
        app.MapGet(basePath + "/documentationforentityandfield/{entity}/{field}", HandleForEntityAndFieldAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<string>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "ForEntityAndField")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query documentation for entity and field");
        
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<DocumentationSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all documentations");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<DocumentationSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapDocumentationServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Documentation",
        string apiOperationPrefix = "Documentation",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        return app;
    }
    
    private static async Task<IResult> HandleForEntityAsync(IPrincipalUtils principalUtils, IDocumentationMetaRepository repository, ClaimsPrincipal user, string entity)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var content = (await repository.ByEntityAndFieldAsync(tenantId, entity, string.Empty))
            ?.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            return Results.Empty;
        }
        
        return Results.Content(content);
    }
    
    private static async Task<IResult> HandleForEntityAndFieldAsync(IPrincipalUtils principalUtils, IDocumentationMetaRepository repository, ClaimsPrincipal user, string entity, string field)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var content = (await repository.ByEntityAndFieldAsync(tenantId, entity, field))
            ?.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            return Results.Empty;
        }
        
        return Results.Content(content);
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, IDocumentationMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, IDocumentationMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectByIdForTenantAsync(tenantId, id));
    }
}