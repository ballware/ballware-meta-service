using System;
using System.Security.Claims;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public class JobCreatePayload
{
    public required string Scheduler { get; set; }
    public required string Identifier { get; set; }
    public required string Options { get; set; }
}

public class JobUpdatePayload
{
    public Guid Id { get; set; }
    public JobStates State { get; set; }
    public string? Result { get; set; }
}

public static class JobMetaEndpoint
{
    private const string ApiTag = "Job";
    private const string ApiOperationPrefix = "Job";
    
    public static IEndpointRouteBuilder MapJobMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/pendingjobsforuser", HandlePendingJobsForUserAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<Job>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "PendingForUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query pending jobs for current user");
        
        return app;
    }

    public static IEndpointRouteBuilder MapJobServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {   
        app.MapPost(basePath + "/createjobfortenantbehalfofuser/{tenantId}/{userId}", HandleCreateJobForTenantBehalfOfUserAsync)
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Produces<Job>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "CreateForTenantBehalfOfUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Create new background job for tenant behalf of user");
        
        app.MapPost(basePath + "/updatejobfortenantbehalfofuser/{tenantId}/{userId}", HandleUpdateJobForTenantBehalfOfUserAsync)
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Produces<Job>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "UpdateForTenantBehalfOfUser")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Update background job for tenant behalf of user");
        
        return app;
    }
    
    private static async Task<IResult> HandlePendingJobsForUserAsync(IPrincipalUtils principalUtils, IJobMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        var userId = principalUtils.GetUserId(user);
        
        return Results.Ok(await repository.PendingJobsForUser(tenantId, userId));
    }
    
    private static async Task<IResult> HandleCreateJobForTenantBehalfOfUserAsync(IJobMetaRepository repository, Guid tenantId, Guid userId, JobCreatePayload data)
    {
        var job = await repository.CreateJobAsync(tenantId, userId, data.Scheduler, data.Identifier, data.Options);

        return Results.Ok(job);
    }
    
    private static async Task<IResult> HandleUpdateJobForTenantBehalfOfUserAsync(IJobMetaRepository repository, Guid tenantId, Guid userId, JobUpdatePayload data)
    {
        var job = await repository.UpdateJobAsync(tenantId, userId, data.Id, data.State, data.Result);

        return Results.Ok(job);
    }
}