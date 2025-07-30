using System;
using System.Collections.Generic;
using System.Security.Claims;
using Ballware.Shared.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class SubscriptionMetaEndpoint
{
    private const string ApiTag = "Subscription";
    private const string ApiOperationPrefix = "Subscription";
    
    public static IEndpointRouteBuilder MapSubscriptionMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<SubscriptionSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all pages");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<SubscriptionSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapSubscriptionServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {
        app.MapGet(basePath + "/metadatabytenantandid/{tenantId}/{id}", HandleMetadataForTenantAndIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Subscription>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "MetadataByTenantAndId")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query subscription metadata by tenant and id");

        app.MapGet(basePath + "/activebytenantandfrequency/{tenantId}/{frequency}", HandleActiveSubscriptionsForTenantAndFrequencyAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<Subscription>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "ActiveByFrequency")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query active subscriptions by frequency");
        
        app.MapPost(basePath + "/setsendresult/{tenantId}/{id}", HandleSetSendResult)
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SetSendResult")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Send send result for subscription");
        
        return app;
    }
    
    private static async Task<IResult> HandleSelectListAsync(IPrincipalUtils principalUtils, ISubscriptionMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForTenantAsync(tenantId));
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(IPrincipalUtils principalUtils, ISubscriptionMetaRepository repository, ClaimsPrincipal user, Guid id)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        
        var entry = await repository.SelectByIdForTenantAsync(tenantId, id);
        
        if (entry == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleMetadataForTenantAndIdAsync(ISubscriptionMetaRepository repository, Guid tenantId, Guid id)
    {
        var subscription = await repository.MetadataByTenantAndIdAsync(tenantId, id);
        
        if (subscription == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(subscription);
    }

    private static async Task<IResult> HandleActiveSubscriptionsForTenantAndFrequencyAsync(ISubscriptionMetaRepository repository,
        Guid tenantId, int frequency)
    {
        var subscriptions = await repository.GetActiveSubscriptionsByTenantAndFrequencyAsync(tenantId, frequency);
        
        return Results.Ok(subscriptions);
    }

    private static async Task<IResult> HandleSetSendResult(ISubscriptionMetaRepository repository, Guid tenantId,
        Guid id, [FromBody] string error)
    {
        await repository.SetLastErrorAsync(tenantId, id, error);

        return Results.Ok();
    }
}