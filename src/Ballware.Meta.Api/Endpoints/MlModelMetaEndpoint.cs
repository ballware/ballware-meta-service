using System.Security.Claims;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class MlModelMetaEndpoint
{
    private const string ApiTag = "MlModel";
    private const string ApiOperationPrefix = "MlModel";
    
    public static IEndpointRouteBuilder MapMlModelMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {   
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<MlModelSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all documents");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<MlModelSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapMlModelServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/metadatabytenantandid/{tenantId}/{id}", HandleMetadataByTenantAndIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<MlModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataByTenantAndId")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query notification metadata by tenant and id");
        
        app.MapGet(basePath + "/metadatabytenantandidentifier/{tenantId}/{identifier}", HandleMetadataByTenantAndIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<MlModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataByTenantAndIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query notification metadata by tenant and identifier");
        
        app.MapPost(basePath + "/savetrainingstatebehalfofuser/{tenantId}/{userId}", HandleSaveTrainingStateBehalfOfUserAsync)
            .RequireAuthorization(authorizationScope)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SaveTrainingStateBehalfOfUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Save training behalf of user");
        
        return app;
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, IMlModelMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, IMlModelMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectByIdForTenantAsync(tenantId, id));
    }
    
    private static async Task<IResult> HandleMetadataByTenantAndIdAsync(ITenantMetaRepository tenantMetaRepository, IMlModelMetaRepository repository, Guid tenantId, Guid id)
    {
        var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
        
        if (tenant == null)
        {
            return Results.NotFound("Tenant not found");
        }
        
        var model = await repository.MetadataByTenantAndIdAsync(tenant, id);
        
        return Results.Ok(model);
    }
    
    private static async Task<IResult> HandleMetadataByTenantAndIdentifierAsync(ITenantMetaRepository tenantMetaRepository, IMlModelMetaRepository repository, Guid tenantId, string identifier)
    {
        var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
        
        if (tenant == null)
        {
            return Results.NotFound("Tenant not found");
        }
        
        var model = await repository.MetadataByTenantAndIdentifierAsync(tenant, identifier);
        
        return Results.Ok(model);
    }
    
    private static async Task<IResult> HandleSaveTrainingStateBehalfOfUserAsync(ITenantMetaRepository tenantMetaRepository, IMlModelMetaRepository repository, Guid tenantId, Guid userId, MlModelTrainingState trainingState)
    {
        var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
        
        if (tenant == null)
        {
            return Results.NotFound("Tenant not found");
        }
        
        await repository.SaveTrainingStateAsync(tenant, userId, trainingState);
        
        return Results.Ok();
    }
}