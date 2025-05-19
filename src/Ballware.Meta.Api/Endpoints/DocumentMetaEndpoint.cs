using System;
using System.Collections.Immutable;
using System.Security.Claims;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class DocumentMetaEndpoint
{
    private const string ApiTag = "Document";
    private const string ApiOperationPrefix = "Document";
    
    public static IEndpointRouteBuilder MapDocumentMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/selectlistdocumentsforentity/{entity}", HandleSelectListForEntityAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<DocumentSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListForEntity")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query available documents for entity");
        
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<DocumentSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all documents");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<DocumentSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapDocumentServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/selectlistdocumentsfortenant/{tenantId}", HandleSelectListForTenantAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<DocumentSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListForTenant")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query available documents for tenant");
        
        app.MapGet(basePath + "/documentmetadatabytenantandid/{tenantId}/{id}", HandleMetadataForTenantAndIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Document>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataForTenantAndId")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query document metadata by tenant and id");
        
        app.MapGet(basePath + "/documenttemplatebehalfofuserbytenant/{tenantId}", HandleNewForTenantAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Document>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "NewForTenantAndUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query new document template for tenant");
        
        app.MapPost(basePath + "/savedocumentbehalfofuser/{tenantId}/{userId}", HandleSaveForTenantBehalfOfUserAsync)
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SaveForTenantBehalfOfUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Save document behalf of user");
        
        return app;
    }
    
    public static async Task<IResult> HandleSelectListForEntityAsync(IPrincipalUtils principalUtils, ITenantRightsChecker tenantRightsChecker, ITenantMetaRepository tenantMetaRepository, IDocumentMetaRepository repository, ClaimsPrincipal user, string entity)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        var claims = principalUtils.GetUserClaims(user);

        var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
        
        if (tenant == null)
        {
            return Results.NotFound("Tenant not found");
        }
        
        var documentList = (await repository.SelectListForTenantAndEntityAsync(tenantId, entity))
            .ToAsyncEnumerable()
            .WhereAwait(async d =>
                await tenantRightsChecker.HasRightAsync(tenant, "meta", "document", claims,
                    $"visiblestate.{d.State}"))
            .ToEnumerable();

        return Results.Ok(documentList);
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, IDocumentMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, IDocumentMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectByIdForTenantAsync(tenantId, id));
    }
    
    private static async Task<IResult> HandleSelectListForTenantAsync(IDocumentMetaRepository repository, Guid tenantId)
    {
        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleMetadataForTenantAndIdAsync(IDocumentMetaRepository repository, Guid tenantId, Guid id)
    {
        return Results.Ok(await repository.MetadataByTenantAndIdAsync(tenantId, id));
    }
    
    private static async Task<IResult> HandleNewForTenantAsync(IDocumentMetaRepository repository, Guid tenantId)
    {
        return Results.Ok(await repository.NewAsync(tenantId, "primary", ImmutableDictionary<string, object>.Empty));
    }
    
    private static async Task<IResult> HandleSaveForTenantBehalfOfUserAsync(IDocumentMetaRepository repository, Guid tenantId, Guid userId, [FromBody] Document payload)
    {
        await repository.SaveAsync(tenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, payload);
            
        return Results.Ok();
    }
}