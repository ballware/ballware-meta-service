using System;
using System.Collections.Generic;
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

public static class ProcessingStateMetaEndpoint
{
    public static IEndpointRouteBuilder MapProcessingStateMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "ProcessingState",
        string apiOperationPrefix = "ProcessingState",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
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
        string apiTag = "ProcessingState",
        string apiOperationPrefix = "ProcessingState",
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
    
    public static async Task<IResult> HandleSelectListForEntityByIdentifierAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        try
        {
            return Results.Ok(await repository.SelectListForEntityAsync(tenantId, identifier));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleSelectByStateForEntityByIdentifierAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier, int state)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        try
        {
            return Results.Ok(await repository.SelectByStateAsync(tenantId, identifier, state));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleSelectByStateForTenantAndEntityByIdentifierAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, Guid tenantId, string identifier, int state)
    {
        try
        {
            return Results.Ok(await repository.SelectByStateAsync(tenantId, identifier, state));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleSelectListAllSuccessorsForEntityByIdentifierAndStateAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier, int state)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        try
        {
            return Results.Ok(await repository.SelectListPossibleSuccessorsForEntityAsync(tenantId, identifier, state));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleSelectListAllSuccessorsForTenantAndEntityByStateAsync(IProcessingStateMetaRepository repository, Guid tenantId, string identifier, int state)
    {
        try
        {
            return Results.Ok(await repository.SelectListPossibleSuccessorsForEntityAsync(tenantId, identifier, state));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
}