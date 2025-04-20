using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class MlModelMetaEndpoint
{
    public static IEndpointRouteBuilder MapMlModelMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "MlModel",
        string apiOperationPrefix = "MlModel",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {   
        return app;
    }

    public static IEndpointRouteBuilder MapMlModelServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "MlModel",
        string apiOperationPrefix = "MlModel",
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
    
    public static async Task<IResult> HandleMetadataByTenantAndIdAsync(IPrincipalUtils principalUtils, ITenantMetaRepository tenantMetaRepository, IMlModelMetaRepository repository, ClaimsPrincipal user, Guid tenantId, Guid id)
    {
        try
        {
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
            var model = await repository.MetadataByTenantAndIdAsync(tenant, id);
            
            return Results.Ok(model);
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleMetadataByTenantAndIdentifierAsync(IPrincipalUtils principalUtils, ITenantMetaRepository tenantMetaRepository, IMlModelMetaRepository repository, ClaimsPrincipal user, Guid tenantId, string identifier)
    {
        try
        {
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
            var model = await repository.MetadataByTenantAndIdentifierAsync(tenant, identifier);
            
            return Results.Ok(model);
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleSaveTrainingStateBehalfOfUserAsync(ITenantMetaRepository tenantMetaRepository, IMlModelMetaRepository repository, Guid tenantId, Guid userId, MlModelTrainingState trainingState)
    {
        try
        {
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
            await repository.SaveTrainingStateAsync(tenant, userId, trainingState);
            
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
}