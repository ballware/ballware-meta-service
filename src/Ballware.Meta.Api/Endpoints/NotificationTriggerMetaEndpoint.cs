using System.Collections.Immutable;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public class NotificationTriggerCreatePayload
{
    public required Guid NotificationId { get; set; }
    public string? Params { get; set; }
}

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
        app.MapPost(basePath + "/createnotificationtriggerfortenantbehalfofuser/{tenantId}/{userId}", HandleCreateForTenantAndNotificationBehalfOfUserAsync)
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "CreateForTenantBehalfOfUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Create new notification trigger for tenant behalf of user");
        
        return app;
    }
    
    private static async Task<IResult> HandleCreateForTenantAndNotificationBehalfOfUserAsync(INotificationTriggerMetaRepository repository, Guid tenantId, Guid userId, NotificationTriggerCreatePayload payload)
    {
        var notificationTrigger = await repository.NewAsync(tenantId, "primary", ImmutableDictionary<string, object>.Empty);
        
        notificationTrigger.NotificationId = payload.NotificationId;
        notificationTrigger.Params = payload.Params;
        
        await repository.SaveAsync(tenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, notificationTrigger);
        
        return Results.Ok();
    }
}