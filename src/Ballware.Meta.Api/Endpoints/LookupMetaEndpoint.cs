using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class LookupMetaEndpoint
{
    public static IEndpointRouteBuilder MapLookupMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Lookup",
        string apiOperationPrefix = "Lookup",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        return app;
    }

    public static IEndpointRouteBuilder MapLookupServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Lookup",
        string apiOperationPrefix = "Lookup",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/lookupmetadatabytenantandidentifier/{tenantId}/{identifier}", HandleMetadataForTenantAndIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Lookup>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataForTenantAndIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query lookup metadata by tenant and identifier");
        
        return app;
    }
    
    public static async Task<IResult> HandleMetadataForTenantAndIdentifierAsync(ILookupMetaRepository repository, Guid tenantId, string identifier)
    {
        try
        {
            var lookup = await repository.ByIdentifierAsync(tenantId, identifier);

            return Results.Ok(lookup);
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
}