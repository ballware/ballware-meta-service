using System;
using System.Collections.Generic;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class SubscriptionMetaEndpoint
{
    public static IEndpointRouteBuilder MapSubscriptionMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Subscription",
        string apiOperationPrefix = "Subscription",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        return app;
    }

    public static IEndpointRouteBuilder MapSubscriptionServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Subscription",
        string apiOperationPrefix = "Subscription",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {
        app.MapGet(basePath + "/metadatabytenantandid/{tenantId}/{id}", HandleMetadataForTenantAndIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Subscription>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataByTenantAndId")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query subscription metadata by tenant and id");

        app.MapGet(basePath + "/activeforfrequency/{frequency}", HandleActiveSubscriptionsByFrequency)
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
    
    public static async Task<IResult> HandleMetadataForTenantAndIdAsync(ISubscriptionMetaRepository repository, Guid tenantId, Guid id)
    {
        var subscription = await repository.MetadataByTenantAndIdAsync(tenantId, id);
        
        return Results.Ok(subscription);
    }

    public static async Task<IResult> HandleActiveSubscriptionsByFrequency(ISubscriptionMetaRepository repository,
        int frequency)
    {
        var subscriptions = await repository.GetActiveSubscriptionsByFrequencyAsync(frequency);
        
        return Results.Ok(subscriptions);
    }

    public static async Task<IResult> HandleSetSendResult(ISubscriptionMetaRepository repository, Guid tenantId,
        Guid id, [FromBody] string error)
    {
        await repository.SetLastErrorAsync(tenantId, id, error);

        return Results.Ok();
    }
}