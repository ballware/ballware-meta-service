using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class EntityMetaEndpoint
{
    private const string ApiTag = "Entity";
    private const string ApiOperationPrefix = "Entity";
    
    public static IEndpointRouteBuilder MapEntityMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/metadataforentity/{identifier}", HandleMetadataByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<MetaEntity>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "MetadataByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query metadata for entity");
        
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<EntitySelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all entities");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<EntitySelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        app.MapGet(basePath + "/selectbyidentifier/{identifier}", HandleSelectByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<EntitySelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "SelectByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        app.MapGet(basePath + "/selectlistrights", HandleSelectListRightsAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<EntityRightSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListRights")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all defined entity rights");
        
        return app;
    }

    public static IEndpointRouteBuilder MapEntityServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/servicemetadatafortenantbyidentifier/{tenantId}/{identifier}", HandleServiceMetadataByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<ServiceEntity>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "ServiceMetadataForTenantByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query metadata for entity");
        
        return app;
    }
    
    internal static async Task<IResult> HandleMetadataByIdentifierAsync(IMapper mapper, IPrincipalUtils principalUtils, IEntityMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var entry = await repository.ByEntityAsync(tenantId, identifier);

        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(mapper.Map<MetaEntity>(entry));
    }
    
    internal static async Task<IResult> HandleServiceMetadataByIdentifierAsync(IMapper mapper, IEntityMetaRepository repository, Guid tenantId, string identifier)
    {
        var entry = await repository.ByEntityAsync(tenantId, identifier);

        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(mapper.Map<ServiceEntity>(entry));
    }
    
    internal static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, IEntityMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    internal static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, IEntityMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var entry = await repository.SelectByIdForTenantAsync(tenantId, id);

        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    internal static async Task<IResult> HandleSelectByIdentifierAsync(IPrincipalUtils principalUtils, IEntityMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var entry = await repository.SelectByIdentifierForTenantAsync(tenantId, identifier);

        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    internal static async Task<IResult> HandleSelectListRightsAsync(IPrincipalUtils principalUtils, IEntityMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListEntityRightsForTenantAsync(tenantId));
    }
}