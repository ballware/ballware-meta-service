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
    public static IEndpointRouteBuilder MapNotificationMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Notification",
        string apiOperationPrefix = "Notification",
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
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapNotificationServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Notification",
        string apiOperationPrefix = "Notification",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/notificationmetadatabytenantandid/{tenantId}/{id}", HandleMetadataByTenantAndIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Notification>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataByTenantAndId")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query notification metadata by tenant and id");
        
        app.MapGet(basePath + "/notificationmetadatabytenantandidentifier/{tenantId}/{identifier}", HandleMetadataByTenantAndIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Notification>()
            .Produces(StatusCodes.Status401Unauthorized)
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

        return Results.Ok(await repository.SelectByIdForTenantAsync(tenantId, id));
    }
    
    private static async Task<IResult> HandleMetadataByTenantAndIdAsync(IPrincipalUtils principalUtils, INotificationMetaRepository repository, ClaimsPrincipal user, Guid tenantId, Guid id)
    {
        return Results.Ok(await repository.MetadataByTenantAndIdAsync(tenantId, id));
    }
    
    private static async Task<IResult> HandleMetadataByTenantAndIdentifierAsync(IPrincipalUtils principalUtils, INotificationMetaRepository repository, ClaimsPrincipal user, Guid tenantId, string identifier)
    {
        return Results.Ok(await repository.MetadataByTenantAndIdentifierAsync(tenantId, identifier));
    }
}