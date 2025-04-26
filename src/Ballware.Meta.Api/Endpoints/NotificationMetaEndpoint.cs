using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
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
    
    public static async Task<IResult> HandleMetadataByTenantAndIdAsync(IPrincipalUtils principalUtils, INotificationMetaRepository repository, ClaimsPrincipal user, Guid tenantId, Guid id)
    {
        try
        {
            return Results.Ok(await repository.MetadataByTenantAndIdAsync(tenantId, id));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleMetadataByTenantAndIdentifierAsync(IPrincipalUtils principalUtils, INotificationMetaRepository repository, ClaimsPrincipal user, Guid tenantId, string identifier)
    {
        try
        {
            return Results.Ok(await repository.MetadataByTenantAndIdentifierAsync(tenantId, identifier));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
}