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

public static class PageMetaEndpoint
{
    public static IEndpointRouteBuilder MapPageMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Page",
        string apiOperationPrefix = "Page",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/pagedataforidentifier/{identifier}", HandleMetadataByIdentifierAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<Page>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "MetadataForPageByIdentifier")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query metadata for page by identifier");
        
        return app;
    }

    public static IEndpointRouteBuilder MapPageServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Page",
        string apiOperationPrefix = "Page",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        return app;
    }
    
    public static async Task<IResult> HandleMetadataByIdentifierAsync(IPrincipalUtils principalUtils, IPageMetaRepository repository, ClaimsPrincipal user, string identifier)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        try
        {
            return Results.Ok(await repository.ByIdentifierAsync(tenantId, identifier));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
}