using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MimeTypes;
using Quartz;

namespace Ballware.Meta.Api.Endpoints;

public static class EndpointHandlerFactory
{
    private static readonly string DefaultQuery = "primary";
    private static readonly string RightView = "view";
    private static readonly string RightAdd = "add";
    private static readonly string RightEdit = "edit";
    private static readonly string RightDelete = "delete";

    public delegate Task<IResult> HandleAllDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, IRepository<TEntity> repository, ClaimsPrincipal user, string identifier) where TEntity : class;

    public delegate Task<IResult> HandleNewDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, IRepository<TEntity> repository, ClaimsPrincipal user, string identifier) where TEntity : class;
    
    public delegate Task<IResult> HandleByIdDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, IRepository<TEntity> repository, ClaimsPrincipal user, string identifier, Guid id) where TEntity : class;
    
    public delegate Task<IResult> HandleSaveDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, IRepository<TEntity> repository, ClaimsPrincipal user, string identifier, TEntity value) where TEntity : class;
    
    public delegate Task<IResult> HandleRemoveDelegate<TEntity>(IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker,
        ITenantMetaRepository tenantMetaRepository, IRepository<TEntity> repository, ClaimsPrincipal user, Guid id) where TEntity : class;

    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
    public delegate Task<IResult> HandleImportDelegate(ISchedulerFactory schedulerFactory,
        IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker, IJobMetaRepository jobMetaRepository,
        ITenantMetaRepository tenantMetaRepository, ClaimsPrincipal user, IMetaFileStorageAdapter storageAdapter, string identifier,
        IFormFileCollection files);

    public delegate Task<IResult> HandleExportDelegate<TEntity>(
        IPrincipalUtils principalUtils, ITenantRightsChecker rightsChecker, ITenantMetaRepository tenantMetaRepository, 
        IRepository<TEntity> repository, ClaimsPrincipal user, string identifier, [FromBody] IDictionary<string, StringValues> query) 
        where TEntity : class;
    
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
    public delegate Task<IResult> HandleExportUrlDelegate<TEntity>(IPrincipalUtils principalUtils, 
        ITenantRightsChecker rightsChecker, ITenantMetaRepository tenantMetaRepository, 
        IExportMetaRepository exportMetaRepository, IRepository<TEntity> repository, IMetaFileStorageAdapter storageAdapter, 
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
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, RightView);
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }
            
            return Results.Ok(await repository.AllAsync(identifier, claims));
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

            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, RightAdd);

            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(await repository.NewAsync(identifier, claims));
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

            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, RightView);

            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(await repository.ByIdAsync(identifier, claims, id));
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
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, identifier == DefaultQuery ? RightEdit : identifier);
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            await repository.SaveAsync(currentUserId, identifier, claims, value);

            return Results.Ok();
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
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, RightDelete);
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            var removeResult = await repository.RemoveAsync(currentUserId,claims, ImmutableDictionary.CreateRange(new []
            {
                new KeyValuePair<string, object>("Id", id),
            }));

            if (!removeResult.Result)
            {
                return Results.BadRequest(new Exception(string.Join("\r\n", removeResult.Messages)));
            }

            return Results.Ok();
        };
    }
    
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
    public static HandleImportDelegate CreateImportHandler(string application, string entity)
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
            
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, identifier == DefaultQuery ? RightEdit : identifier);
            
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }

            foreach (var file in files)
            {
                var jobData = new JobDataMap();

                jobData["tenantId"] = tenantId;
                jobData["userId"] = currentUserId;
                jobData["identifier"] = identifier;
                jobData["claims"] = JsonSerializer.Serialize(claims);
                jobData["filename"] = file.FileName;

                await storageAdapter.UploadFileForOwnerAsync(currentUserId.ToString(), file.FileName,
                    file.ContentType, file.OpenReadStream());

                var job = await jobMetaRepository.CreateJobAsync(tenant, currentUserId, "generic",
                    "import", JsonSerializer.Serialize(jobData));

                jobData["jobId"] = job.Id;

                await (await schedulerFactory.GetScheduler()).TriggerJob(JobKey.Create("import", entity), jobData);
            }

            return Results.Created();
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
        
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, identifier);
        
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }
        
            var export = await repository.ExportAsync(identifier, claims, queryParams);
        
            return Results.Content(Encoding.UTF8.GetString(export.Data), export.MediaType);
        }; 
    }
    
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
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
        
            var tenantAuthorized = await rightsChecker.HasRightAsync(tenant, application, entity, claims, identifier);
        
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }
        
            var export = await repository.ExportAsync(identifier, claims, queryParams);
        
            var exportEntry = await exportMetaRepository.NewAsync(tenantId, DefaultQuery, claims);

            exportEntry.Entity = entity;
            exportEntry.Query = identifier;
            exportEntry.MediaType = export.MediaType;
            exportEntry.ExpirationStamp = DateTime.Now.AddDays(1);

            await storageAdapter.UploadFileForOwnerAsync("export", $"{exportEntry.Id}{MimeTypeMap.GetExtension(export.MediaType)}", export.MediaType, new MemoryStream(export.Data));
        
            await exportMetaRepository.SaveAsync(tenantId, currentUserId, DefaultQuery, claims, exportEntry);

            return Results.Content(exportEntry.Id.ToString());
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
                queryParams.Add(queryEntry.Key, $"|{string.Join('|', queryEntry.Value.ToArray())}|");
            }
            else
            {
                queryParams.Add(queryEntry.Key, queryEntry.Value);
            }
        }

        return queryParams;
    }
}