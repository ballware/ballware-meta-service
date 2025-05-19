using System;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class NotificationTriggerMetaEndpoint
{
    private const string ApiTag = "NotificationTrigger";
    private const string ApiOperationPrefix = "NotificationTrigger";
    
    public static IEndpointRouteBuilder MapNotificationTriggerMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {   
        return app;
    }

    public static IEndpointRouteBuilder MapNotificationTriggerServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/createnotificationtriggerfortenantandnotificationbehalfofuser/{tenantId}/{notificationId}/{userId}", HandleCreateForTenantAndNotificationBehalfOfUserAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<NotificationTrigger>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "CreateForTenantAndNotificationBehalfOfUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Create new notification trigger for tenant behalf of user");
        
        app.MapPost(basePath + "/savenotificationtriggerbehalfofuser/{tenantId}/{userId}", HandleSaveForTenantBehalfOfUserAsync)
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SaveForTenantBehalfOfUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Save notification trigger behalf of user");
        
        return app;
    }
    
    private static async Task<IResult> HandleCreateForTenantAndNotificationBehalfOfUserAsync(INotificationTriggerMetaRepository repository, Guid tenantId, Guid notificationId, Guid userId)
    {
        var notificationTrigger = await repository.NewAsync(tenantId, "primary", ImmutableDictionary<string, object>.Empty);
        
        notificationTrigger.NotificationId = notificationId;
        
        return Results.Ok(notificationTrigger);
    }
    
    private static async Task<IResult> HandleSaveForTenantBehalfOfUserAsync(INotificationTriggerMetaRepository repository, Guid tenantId, Guid userId, NotificationTrigger payload)
    {
        await repository.SaveAsync(tenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, payload);
        
        return Results.Ok();
    }
}