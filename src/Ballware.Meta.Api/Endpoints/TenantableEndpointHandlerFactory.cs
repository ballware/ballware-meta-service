using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Security.Claims;
using System.Text;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MimeTypes;
using Newtonsoft.Json;
using Quartz;

namespace Ballware.Meta.Api.Endpoints;

public static class TenantableEndpointHandlerFactory
{
    public static readonly string DefaultQuery = "primary";

    public delegate Task<IResult> HandleAllDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier) where TEntity : class;

    public delegate Task<IResult> HandleQueryDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier, QueryValueBag query) where TEntity : class;
    
    public delegate Task<IResult> HandleNewDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier) where TEntity : class;
    
    public delegate Task<IResult> HandleByIdDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier, Guid id) where TEntity : class;
    
    public delegate Task<IResult> HandleSaveDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier, TEntity value) where TEntity : class;
    
    public delegate Task<IResult> HandleRemoveDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, Guid id) where TEntity : class;

    public delegate Task<IResult> HandleImportDelegate<TEntity>(ISchedulerFactory schedulerFactory,
        IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker, IJobMetaRepository jobMetaRepository,
        ITenantMetaRepository tenantMetaRepository, ClaimsPrincipal user, IMetaFileStorageAdapter storageAdapter, string identifier,
        IFormFileCollection files) where TEntity : class;

    public delegate Task<IResult> HandleExportDelegate<TEntity>(
        IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker, ITenantMetaRepository tenantMetaRepository, 
        ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier, [FromBody] IDictionary<string, StringValues> query) 
        where TEntity : class;
    
    public delegate Task<IResult> HandleExportUrlDelegate<TEntity>(IPrincipalUtils principalUtils, 
        ITenantRightsChecker rightsChecker, ITenantMetaRepository tenantMetaRepository, 
        IExportMetaRepository exportMetaRepository, ITenantableRepository<TEntity> repository, IMetaFileStorageAdapter storageAdapter, 
        ClaimsPrincipal user, string identifier, HttpRequest request)
        where TEntity : class;
    
    public delegate Task<IResult> HandleDownloadExportDelegate(IExportMetaRepository exportMetaRepository, 
        IMetaFileStorageAdapter storageAdapter, Guid id);
    
    public static HandleAllDelegate<TEntity> CreateAllHandler<TEntity>(string application, string entity) where TEntity : class 
    {
        return async (principalUtils, rightsChecker, tenantMetaRepository, repository, user, identifier) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, "view");
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }
            
            try
            {
                return Results.Ok(await repository.AllAsync(tenantId, identifier, claims));
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
            }
        };
    }
    
    public static HandleQueryDelegate<TEntity> CreateQueryHandler<TEntity>(string application, string entity) where TEntity : class 
    {
        return async (principalUtils, rightsChecker, tenantMetaRepository, repository, user, identifier, query) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);
            var queryParams = GetQueryParams(query.Query);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, "view");
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }
            
            try
            {
                return Results.Ok(await repository.QueryAsync(tenantId, identifier, claims, queryParams));
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
            }
        };
    }

    public static HandleNewDelegate<TEntity> CreateNewHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, rightsChecker, tenantMetaRepository,
            repository, user, identifier) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);

            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }

            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, "add");

            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            try
            {
                return Results.Ok(await repository.NewAsync(tenantId, identifier, claims));
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message,
                    detail: ex.StackTrace);
            }
        };
    }

    public static HandleByIdDelegate<TEntity> CreateByIdHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, rightsChecker, tenantMetaRepository,
            repository, user, identifier, id) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);

            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }

            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, "view");

            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            try
            {
                return Results.Ok(await repository.ByIdAsync(tenantId, identifier, claims, id));
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message,
                    detail: ex.StackTrace);
            }
        };
    }
    
    public static HandleSaveDelegate<TEntity> CreateSaveHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, rightsChecker, tenantMetaRepository,
            repository, user, identifier, value) =>
        {
            var currentUserId = principalUtils.GetUserId(user);
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, identifier == DefaultQuery ? "edit" : identifier);
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            try
            {
                await repository.SaveAsync(tenantId, currentUserId, identifier, claims, value);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
            }
        };
    }
    
    public static HandleRemoveDelegate<TEntity> CreateRemoveHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, rightsChecker, tenantMetaRepository,
            repository, user, id) =>
        {
            var currentUserId = principalUtils.GetUserId(user);
            var tenantId = principalUtils.GetUserTenandId(user);
            var claims = principalUtils.GetUserClaims(user);

            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, "delete");
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            try
            {
                var removeResult = await repository.RemoveAsync(tenantId, currentUserId,claims, ImmutableDictionary.CreateRange(new []
                {
                    new KeyValuePair<string, object>("Id", id),
                }));

                if (!removeResult.Result)
                {
                    return Results.BadRequest(new Exception(string.Join("\r\n", removeResult.Messages)));
                }

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
            }
        };
    }
    
    public static HandleImportDelegate<TEntity> CreateImportHandler<TEntity>(string application, string entity) where TEntity: class
    {
        return async (schedulerFactory, principalUtils, rightsChecker, jobMetaRepository, tenantMetaRepository, user, storageAdapter, identifier, files) =>
        {
            var currentUserId = principalUtils.GetUserId(user);
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, identifier == DefaultQuery ? "edit" : identifier);
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            try
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        var jobData = new JobDataMap();

                        jobData["tenantId"] = tenantId;
                        jobData["userId"] = currentUserId;
                        jobData["identifier"] = identifier;
                        jobData["claims"] = JsonConvert.SerializeObject(claims);
                        jobData["filename"] = file.FileName;

                        await storageAdapter.UploadFileForOwnerAsync(currentUserId.ToString(), file.FileName, file.ContentType, file.OpenReadStream());

                        var job = await jobMetaRepository.CreateJobAsync(tenant, currentUserId, "generic",
                            "import", JsonConvert.SerializeObject(jobData));

                        jobData["jobId"] = job.Id;

                        await (await schedulerFactory.GetScheduler()).TriggerJob(JobKey.Create("import", entity), jobData);
                    }
                }

                return Results.Created();
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
            }
        };        
    }
    
    public static HandleExportDelegate<TEntity> CreateExportHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, rightsChecker, tenantMetaRepository,
            repository, user, identifier, [FromBody] query) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            var queryParams = GetQueryParams(query);
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }
        
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, identifier ?? "export");
        
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }
        
            try
            {
                var export = await repository.ExportAsync(tenantId, identifier ?? "export", claims, queryParams);
            
                return Results.Content(Encoding.UTF8.GetString(export.Data), export.MediaType);
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
            }
        }; 
    }
    
    public static HandleExportUrlDelegate<TEntity> CreateExportUrlHandler<TEntity>(string application, string entity)
        where TEntity : class
    {
        return async (principalUtils, rightsChecker, tenantMetaRepository, exportMetaRepository, repository, storageAdapter, user, identifier, request) =>
        {
            var query = await request.ReadFormAsync();
            var currentUserId = principalUtils.GetUserId(user);
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
        
            var queryParams = new Dictionary<string, object>();

            foreach (var queryEntry in query)
            {
                queryParams.Add(queryEntry.Key, queryEntry.Value);
            }
        
            var tenant = await tenantMetaRepository.ByIdAsync(tenantId);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {tenantId} not found");
            }
        
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, identifier ?? "export");
        
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }
        
            try
            {
                var export = await repository.ExportAsync(tenantId, identifier ?? "export", claims, queryParams);
            
                var exportEntry = await exportMetaRepository.NewAsync(tenantId, DefaultQuery, claims);

                exportEntry.Entity = entity;
                exportEntry.Query = identifier;
                exportEntry.MediaType = export.MediaType;
                exportEntry.ExpirationStamp = DateTime.Now.AddDays(1);

                await storageAdapter.UploadFileForOwnerAsync("export", $"{exportEntry.Id}{MimeTypeMap.GetExtension(export.MediaType)}", export.MediaType, new MemoryStream(export.Data));
            
                await exportMetaRepository.SaveAsync(tenantId, currentUserId, DefaultQuery, claims, exportEntry);

                return Results.Content(exportEntry.Id.ToString());
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message, detail: ex.StackTrace);
            }
        };
    }

    public static HandleDownloadExportDelegate CreateDownloadExportHandler()
    {
        return async (exportMetaRepository, storageAdapter, id) =>
        {
            var export = await exportMetaRepository.ByIdAsync(id);

            if (export == null || export.ExpirationStamp <= DateTime.Now)
            {
                return Results.NotFound();
            }

            var fileContent = await storageAdapter.FileByNameForOwnerAsync("export", $"{export.Id}{MimeTypeMap.GetExtension(export.MediaType)}");

            return Results.File(fileContent, export.MediaType, $"{export.Query}_{DateTime.Now:yyyyMMdd_HHmmss}{MimeTypeMap.GetExtension(export.MediaType)}");
        };
    }
    
    private static Dictionary<string, object> GetQueryParams(IDictionary<string, StringValues> query)
    {
        var queryParams = new Dictionary<string, object>();

        foreach (var queryEntry in query)
        {
            if (queryEntry.Value.Count > 1)
            {
                queryParams.Add(queryEntry.Key, $"|{string.Join('|', queryEntry.Value)}|");
            }
            else
            {
                queryParams.Add(queryEntry.Key, queryEntry.Value);
            }
        }

        return queryParams;
    }
}