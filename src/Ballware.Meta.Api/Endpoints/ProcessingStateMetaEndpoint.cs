using System.Security.Claims;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class ProcessingStateMetaEndpoint
{
    private const string ApiTag = "ProcessingState";
    private const string ApiOperationPrefix = "ProcessingState";
    
    public static IEndpointRouteBuilder MapProcessingStateMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<ProcessingStateSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query all processing states");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<ProcessingStateSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query single processing state by id");
        
        app.MapGet(basePath + "/selectlistforentity/{identifier}", HandleSelectListForEntityByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<ProcessingStateSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListForEntityByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query all processing states for entity by identifier");
        
        app.MapGet(basePath + "/selectbystateforentity/{identifier}/{state}", HandleSelectByStateForEntityByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<ProcessingStateSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectByStateForEntityByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query single processing state for entity by identifier and state");
        
        app.MapGet(basePath + "/selectlistallsuccessorsforentityandstate/{identifier}/{state}", HandleSelectListAllSuccessorsForEntityByIdentifierAndStateAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<ProcessingStateSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListAllSuccessorsForEntityByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query all possible successor processing states for entity by identifier and current state");
        
        return app;
    }

    public static IEndpointRouteBuilder MapProcessingStateServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/selectlistallsuccessorsfortenantandentitybystate/{tenantId}/{identifier}/{state}", HandleSelectListAllSuccessorsForTenantAndEntityByStateAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<ProcessingStateSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListAllSuccessorsForTenantAndEntityByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query all possible successor processing states for entity by identifier and current state");
        
        app.MapGet(basePath + "/selectbystatefortenantandentity/{tenantId}/{identifier}/{state}", HandleSelectByStateForTenantAndEntityByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<ProcessingStateSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectByStateForTenantAndEntityByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query single processing state for entity by identifier and state");
        
        return app;
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectByIdForTenantAsync(tenantId, id));
    }
    
    private static async Task<IResult> HandleSelectListForEntityByIdentifierAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForEntityAsync(tenantId, identifier));
    }
    
    private static async Task<IResult> HandleSelectByStateForEntityByIdentifierAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier, int state)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectByStateAsync(tenantId, identifier, state));
    }
    
    private static async Task<IResult> HandleSelectByStateForTenantAndEntityByIdentifierAsync(IProcessingStateMetaRepository repository, Guid tenantId, string identifier, int state)
    {
        return Results.Ok(await repository.SelectByStateAsync(tenantId, identifier, state));
    }
    
    private static async Task<IResult> HandleSelectListAllSuccessorsForEntityByIdentifierAndStateAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier, int state)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListPossibleSuccessorsForEntityAsync(tenantId, identifier, state));
    }
    
    private static async Task<IResult> HandleSelectListAllSuccessorsForTenantAndEntityByStateAsync(IProcessingStateMetaRepository repository, Guid tenantId, string identifier, int state)
    {
        return Results.Ok(await repository.SelectListPossibleSuccessorsForEntityAsync(tenantId, identifier, state));
    }
}