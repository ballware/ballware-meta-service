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

public static class StatisticMetaEndpoint
{
    public static IEndpointRouteBuilder MapStatisticMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Statistic",
        string apiOperationPrefix = "Statistic",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/metadataforidentifier", HandleMetadataByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Statistic>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query metadata for statistic by identifier");
        
        return app;
    }

    public static IEndpointRouteBuilder MapStatisticServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Statistic",
        string apiOperationPrefix = "Statistic",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        return app;
    }
    
    public static async Task<IResult> HandleMetadataByIdentifierAsync(IPrincipalUtils principalUtils, IStatisticMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        try
        {
            return Results.Ok(await repository.MetadataByIdentifierAsync(tenantId, identifier));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
}