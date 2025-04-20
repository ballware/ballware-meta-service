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

public class JobCreatePayload
{
    public string Scheduler { get; set; }
    public string Identifier { get; set; }
    public string Options { get; set; }
}

public class JobUpdatePayload
{
    public Guid Id { get; set; }
    public JobStates State { get; set; }
    public string Result { get; set; }
}

public static class JobMetaEndpoint
{
    public static IEndpointRouteBuilder MapJobMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = "Job",
        string apiOperationPrefix = "Job",
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
        string apiTag = "Job",
        string apiOperationPrefix = "Job",
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
    
    public static async Task<IResult> HandlePendingJobsForUserAsync(IPrincipalUtils principalUtils, ITenantMetaRepository tenantMetaRepository, IJobMetaRepository repository, ClaimsPrincipal user)
    {
        var tenantId = principalUtils.GetUserTenandId(user);
        var userId = principalUtils.GetUserId(user);

        try
        {
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            return Results.Ok(await repository.PendingJobsForUser(tenant, userId));
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleCreateJobForTenantBehalfOfUserAsync(IPrincipalUtils principalUtils, ITenantMetaRepository tenantMetaRepository, IJobMetaRepository repository, ClaimsPrincipal user, Guid tenantId, Guid userId, JobCreatePayload data)
    {
        try
        {
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
        
            var job = await repository.CreateJobAsync(tenant, userId, data.Scheduler, data.Identifier, data.Options);

            return Results.Ok(job);
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
    
    public static async Task<IResult> HandleUpdateJobForTenantBehalfOfUserAsync(IPrincipalUtils principalUtils, ITenantMetaRepository tenantMetaRepository, IJobMetaRepository repository, ClaimsPrincipal user, Guid tenantId, Guid userId, JobUpdatePayload data)
    {
        try
        {
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
        
            var job = await repository.UpdateJobAsync(tenant, userId, data.Id, data.State, data.Result);

            return Results.Ok(job);
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
        }
    }
}