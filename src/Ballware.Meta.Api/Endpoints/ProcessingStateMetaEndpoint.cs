using System.Security.Claims;
using Ballware.Meta.Api.Bindings;
using Ballware.Shared.Authorization;
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
            .Produces(StatusCodes.Status404NotFound)
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
            .Produces(StatusCodes.Status404NotFound)
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
        
        app.MapGet(basePath + "/selectlistallowedsuccessorsforentities/document", HandleSelectListAllowedSuccessorsForDocumentByIdsAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<ProcessingStateSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListAllSuccessorsForDocumentByIds")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query all possible successor processing states for documents by instance ids");
        
        app.MapGet(basePath + "/selectlistallowedsuccessorsforentities/notification", HandleSelectListAllowedSuccessorsForNotificationByIdsAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<ProcessingStateSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListAllSuccessorsForNotificationByIds")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query all possible successor processing states for notifications by instance ids");
        
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
        
        var entry = await repository.SelectByIdForTenantAsync(tenantId, id);

        if (entry == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleSelectListForEntityByIdentifierAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForEntityAsync(tenantId, identifier));
    }
    
    private static async Task<IResult> HandleSelectByStateForEntityByIdentifierAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier, int state)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var entry = await repository.SelectByStateAsync(tenantId, identifier, state);

        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleSelectByStateForTenantAndEntityByIdentifierAsync(IProcessingStateMetaRepository repository, Guid tenantId, string identifier, int state)
    {
        var entry = await repository.SelectByStateAsync(tenantId, identifier, state);

        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleSelectListAllSuccessorsForEntityByIdentifierAndStateAsync(IPrincipalUtils principalUtils, IProcessingStateMetaRepository repository, ClaimsPrincipal user, string identifier, int state)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListPossibleSuccessorsForEntityAsync(tenantId, identifier, state));
    }
    
    private static async Task<IResult> HandleSelectListAllowedSuccessorsForDocumentByIdsAsync(IPrincipalUtils principalUtils, IEntityRightsChecker entityRightsChecker, IEntityMetaRepository entityMetaRepository, IProcessingStateMetaRepository processingStateMetaRepository, IDocumentMetaRepository documentMetaRepository, ClaimsPrincipal user, QueryValueBag query)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        var rights = principalUtils.GetUserRights(user);

        var entityMeta = await entityMetaRepository.ByEntityAsync(tenantId, "document");

        if (entityMeta == null)
        {
            return Results.NotFound($"Entity document not found.");
        }

        query.Query.TryGetValue("id", out var ids);
        
        var listOfStates = new List<IEnumerable<ProcessingStateSelectListEntry>>();

        foreach (var id in ids.Select(Guid.Parse))
        {
            var currentState = await documentMetaRepository.GetCurrentStateForTenantAndIdAsync(tenantId, id);
            var possibleStates = currentState != null
                ? (await processingStateMetaRepository.SelectListPossibleSuccessorsForEntityAsync(tenantId, "document",
                    currentState.Value)).ToList()
                : [];
            var allowedStates = possibleStates?.Where(ps => entityRightsChecker.StateAllowedAsync(tenantId, entityMeta, id, ps.State, rights).GetAwaiter().GetResult());

            listOfStates.Add(allowedStates);
        }

        if (listOfStates.Count > 1)
        {
            return Results.Ok(listOfStates.Skip(1).Aggregate(new HashSet<ProcessingStateSelectListEntry>(listOfStates[0]), (h, e) =>
            {
                h.IntersectWith(e);
                return h;
            }));
        }
        
        if (listOfStates.Count == 1)
        {
            return Results.Ok(listOfStates[0]);
        }
        
        return Results.Ok(new List<ProcessingStateSelectListEntry>());
    }
    
    private static async Task<IResult> HandleSelectListAllowedSuccessorsForNotificationByIdsAsync(IPrincipalUtils principalUtils, IEntityRightsChecker entityRightsChecker, IEntityMetaRepository entityMetaRepository, IProcessingStateMetaRepository processingStateMetaRepository, INotificationMetaRepository notificationMetaRepository, ClaimsPrincipal user, QueryValueBag query)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        var rights = principalUtils.GetUserRights(user);

        var entityMeta = await entityMetaRepository.ByEntityAsync(tenantId, "notification");

        if (entityMeta == null)
        {
            return Results.NotFound($"Entity notification not found.");
        }

        query.Query.TryGetValue("id", out var ids);
        
        var listOfStates = (await Task.WhenAll(ids.Select(Guid.Parse).Select(async (id) =>
        {
            var currentState = await notificationMetaRepository.GetCurrentStateForTenantAndIdAsync(tenantId, id);
            var possibleStates = currentState != null
                ? await processingStateMetaRepository.SelectListPossibleSuccessorsForEntityAsync(tenantId, "notification",
                    currentState.Value)
                : [];
            var allowedStates = possibleStates?.Where(ps => entityRightsChecker.StateAllowedAsync(tenantId, entityMeta, id, ps.State, rights).GetAwaiter().GetResult());

            return allowedStates;
        })))?.ToList();

        if (listOfStates != null && listOfStates.Count > 1)
        {
            return Results.Ok(listOfStates.Skip(1).Aggregate(new HashSet<ProcessingStateSelectListEntry>(listOfStates[0]), (h, e) =>
            {
                h.IntersectWith(e);
                return h;
            }));
        }
        
        if (listOfStates != null && listOfStates.Count == 1)
        {
            return Results.Ok(listOfStates[0]);
        }
        
        return Results.Ok(new List<ProcessingStateSelectListEntry>());
    }
    
    private static async Task<IResult> HandleSelectListAllSuccessorsForTenantAndEntityByStateAsync(IProcessingStateMetaRepository repository, Guid tenantId, string identifier, int state)
    {
        return Results.Ok(await repository.SelectListPossibleSuccessorsForEntityAsync(tenantId, identifier, state));
    }
}