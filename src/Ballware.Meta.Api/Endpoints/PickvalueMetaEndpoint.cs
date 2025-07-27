using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Ballware.Shared.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class PickvalueMetaEndpoint
{
    private const string ApiTag = "Pickvalue";
    private const string ApiOperationPrefix = "Pickvalue";
    
    public static IEndpointRouteBuilder MapPickvalueMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/selectlistforentityandfield/{entity}/{field}", HandleSelectListForEntityAndFieldAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<PickvalueSelectEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectListForEntityAndField")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of pickvalues for entity and field");
        
        app.MapGet(basePath + "/selectbyvalueforentityandfield/{entity}/{field}/{value}", HandleSelectByValueForEntityAndFieldAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<PickvalueSelectEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "SelectByValueForEntityAndField")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query single pickvalue for entity and field by value");
        
        return app;
    }

    public static IEndpointRouteBuilder MapPickvalueServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapGet(basePath + "/selectbyvaluefortenantandentityandfield/{tenantId}/{entity}/{field}/{value}", HandleSelectByValueForTenantEntityAndFieldAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<PickvalueSelectEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "SelectByValueForTenantEntityAndField")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query single pickvalue for tenant and entity and field by value");
        
        return app;
    }
    
    private static async Task<IResult> HandleSelectListForEntityAndFieldAsync(IPrincipalUtils principalUtils, IPickvalueMetaRepository repository, ClaimsPrincipal user, string entity, string field)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        return Results.Ok(await repository.SelectListForEntityFieldAsync(tenantId, entity, field));
    }
    
    private static async Task<IResult> HandleSelectByValueForEntityAndFieldAsync(IPrincipalUtils principalUtils, IPickvalueMetaRepository repository, ClaimsPrincipal user, string entity, string field, int value)
    {
        var tenantId = principalUtils.GetUserTenandId(user);

        var entry = await repository.SelectByValueAsync(tenantId, entity, field, value);
        
        if (entry == null) 
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleSelectByValueForTenantEntityAndFieldAsync(IPickvalueMetaRepository repository, Guid tenantId, string entity, string field, int value)
    {
        var entry = await repository.SelectByValueAsync(tenantId, entity, field, value);
        
        if (entry == null) 
        {
            return Results.NotFound();
        }
        
        return Results.Ok(entry);
    }
}