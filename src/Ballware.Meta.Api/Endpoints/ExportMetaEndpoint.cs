using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public class ExportCreatePayload
{
    public required string Application { get; set; }

    public required string Entity { get; set; }

    public required string Query { get; set; }

    public System.DateTimeOffset? ExpirationStamp { get; set; }

    public required string MediaType { get; set; }
}

public static class ExportMetaEndpoint
{
    private const string ApiTag = "Export";
    private const string ApiOperationPrefix = "Export";
    
    public static IEndpointRouteBuilder MapExportMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        return app;
    }

    public static IEndpointRouteBuilder MapExportServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/exportbyidfortenant/{tenantId}/{id}", HandleFetchByIdForTenantAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Export>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "FetchForTenantById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Fetch export for tenant by id");
        
        app.MapPost(basePath + "/createexportfortenantbehalfofuser/{tenantId}/{userId}", HandleCreateForTenantBehalfOfUserAsync)
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Produces<Guid>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "CreateForTenantBehalfOfUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Create new export for tenant behalf of user");
        
        return app;
    }
    
    private static async Task<IResult> HandleFetchByIdForTenantAsync(IExportMetaRepository repository, Guid tenantId, Guid id)
    {
        var export = await repository.ByIdAsync(tenantId, id);
        
        return export != null ? Results.Ok(export) : Results.NotFound();
    }
    
    private static async Task<IResult> HandleCreateForTenantBehalfOfUserAsync(IExportMetaRepository repository, Guid tenantId, Guid userId, ExportCreatePayload payload)
    {
        var export = await repository.NewAsync(tenantId, "primary", ImmutableDictionary<string, object>.Empty);
        
        export.Application = payload.Application;
        export.Entity = payload.Entity;
        export.Query = payload.Query;
        export.ExpirationStamp = payload.ExpirationStamp?.DateTime;
        export.MediaType = payload.MediaType;
    
        await repository.SaveAsync(tenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, export);
        
        return Results.Ok(export.Id);
    }
}