using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class DocumentationMetaEndpoint
{
    public static IEndpointRouteBuilder MapDocumentationMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Documentation",
        string apiOperationPrefix = "Documentation",
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/documentationforentity/{entity}", HandleForEntityAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<string>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "ForEntity")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query documentation for entity");
        
        app.MapGet(basePath + "/documentationforentityandfield/{entity}/{field}", HandleForEntityAndFieldAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<string>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "ForEntityAndField")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query documentation for entity and field");
        
        return app;
    }

    public static IEndpointRouteBuilder MapDocumentationServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = "Documentation",
        string apiOperationPrefix = "Documentation",
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        return app;
    }
    
    public static async Task<IResult> HandleForEntityAsync(IPrincipalUtils principalUtils, IDocumentationMetaRepository repository, ClaimsPrincipal user, string entity)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        try
        {
            var content = (await repository.ByEntityAndFieldAsync(tenantId, entity, string.Empty))
                ?.Content;

            if (string.IsNullOrWhiteSpace(content))
            {
                return Results.Empty;
            }
            
            return Results.Content(content);
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleForEntityAndFieldAsync(IPrincipalUtils principalUtils, IDocumentationMetaRepository repository, ClaimsPrincipal user, string entity, string field)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        try
        {
            var content = (await repository.ByEntityAndFieldAsync(tenantId, entity, field))
                ?.Content;

            if (string.IsNullOrWhiteSpace(content))
            {
                return Results.Empty;
            }
            
            return Results.Content(content);
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
}