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

public static class NotificationMetaEndpoint
{
    private const string ApiTag = "Notification";
    private const string ApiOperationPrefix = "Notification";
    
    public static IEndpointRouteBuilder MapNotificationMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {   
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<NotificationSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all notifications");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<NotificationSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapNotificationServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/notificationmetadatabytenantandid/{tenantId}/{id}", HandleMetadataByTenantAndIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Notification>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "MetadataByTenantAndId")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query notification metadata by tenant and id");
        
        app.MapGet(basePath + "/notificationmetadatabytenantandidentifier/{tenantId}/{identifier}", HandleMetadataByTenantAndIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Notification>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "MetadataByTenantAndIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query notification metadata by tenant and identifier");
        
        return app;
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, INotificationMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, INotificationMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        
        var entry = await repository.SelectByIdForTenantAsync(tenantId, id);

        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleMetadataByTenantAndIdAsync(INotificationMetaRepository repository, Guid tenantId, Guid id)
    {
        var entry = await repository.MetadataByTenantAndIdAsync(tenantId, id);
        
        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleMetadataByTenantAndIdentifierAsync(INotificationMetaRepository repository, Guid tenantId, string identifier)
    {
        var entry = await repository.MetadataByTenantAndIdentifierAsync(tenantId, identifier);
        
        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
}