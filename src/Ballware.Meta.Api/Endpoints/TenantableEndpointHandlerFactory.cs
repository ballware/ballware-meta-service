using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Ballware.Meta.Api.Internal;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using MimeTypes;
using Quartz;

namespace Ballware.Meta.Api.Endpoints;

public static class TenantableEndpointHandlerFactory
{
    private static readonly string DefaultQuery = "primary";

    private static readonly string RightView = "view";
    private static readonly string RightAdd = "add";
    private static readonly string RightEdit = "edit";
    private static readonly string RightDelete = "delete";
    
    public delegate Task<IResult> HandleAllDelegate<TEntity>(IPrincipalUtils principalUtils, EditingEndpointBuilderFactory endpointFactory, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier) where TEntity : class;

    public delegate Task<IResult> HandleQueryDelegate<TEntity>(IPrincipalUtils principalUtils, EditingEndpointBuilderFactory endpointFactory, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier, QueryValueBag query) where TEntity : class;
    
    public delegate Task<IResult> HandleNewDelegate<TEntity>(IPrincipalUtils principalUtils, EditingEndpointBuilderFactory endpointFactory, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier) where TEntity : class;
    
    public delegate Task<IResult> HandleByIdDelegate<TEntity>(IPrincipalUtils principalUtils, EditingEndpointBuilderFactory endpointFactory, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier, Guid id) where TEntity : class;
    
    public delegate Task<IResult> HandleSaveDelegate<TEntity>(IPrincipalUtils principalUtils, EditingEndpointBuilderFactory endpointFactory, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier, TEntity value) where TEntity : class;
    
    public delegate Task<IResult> HandleRemoveDelegate<TEntity>(IPrincipalUtils principalUtils, EditingEndpointBuilderFactory endpointFactory, ITenantableRepository<TEntity> repository, ClaimsPrincipal user, Guid id) where TEntity : class;

    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
    public delegate Task<IResult> HandleImportDelegate(ISchedulerFactory schedulerFactory,
        IPrincipalUtils principalUtils, EditingEndpointBuilderFactory endpointFactory, IJobMetaRepository jobMetaRepository,
        ClaimsPrincipal user, IMetaFileStorageAdapter storageAdapter, string identifier,
        IFormFileCollection files);

    public delegate Task<IResult> HandleExportDelegate<TEntity>(
        IPrincipalUtils principalUtils, EditingEndpointBuilderFactory endpointFactory, 
        ITenantableRepository<TEntity> repository, ClaimsPrincipal user, string identifier, HttpRequest request) 
        where TEntity : class;
    
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
    public delegate Task<IResult> HandleExportUrlDelegate<TEntity>(IPrincipalUtils principalUtils, 
        EditingEndpointBuilderFactory endpointFactory, 
        IExportMetaRepository exportMetaRepository, ITenantableRepository<TEntity> repository, IMetaFileStorageAdapter storageAdapter, 
        ClaimsPrincipal user, string identifier, HttpRequest request)
        where TEntity : class;
    
    public delegate Task<IResult> HandleDownloadExportDelegate(IExportMetaRepository exportMetaRepository, 
        IMetaFileStorageAdapter storageAdapter, Guid id);
    
    public static HandleAllDelegate<TEntity> CreateAllHandler<TEntity>(string application, string entity) where TEntity : class 
    {
        return async (principalUtils, endpointFactory, repository, user, identifier) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);
            var claims = principalUtils.GetUserClaims(user);

            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .WithTenantAndEntityRightCheck(RightView, ImmutableDictionary<string, object>.Empty)
                .ExecuteAsync(async () => Results.Ok(await repository.AllAsync(tenantId, identifier, claims)));
        };
    }
    
    public static HandleQueryDelegate<TEntity> CreateQueryHandler<TEntity>(string application, string entity) where TEntity : class 
    {
        return async (principalUtils, endpointFactory, repository, user, identifier, query) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            var queryParams = GetQueryParams(query.Query);
            
            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .WithTenantAndEntityRightCheck(RightView, queryParams)
                .ExecuteAsync(async () => Results.Ok(await repository.QueryAsync(tenantId, identifier, claims, queryParams)));
        };
    }

    public static HandleNewDelegate<TEntity> CreateNewHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, endpointFactory,
            repository, user, identifier) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            
            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .WithTenantAndEntityRightCheck(RightAdd, ImmutableDictionary<string, object>.Empty)
                .ExecuteAsync(async () => Results.Ok(await repository.NewAsync(tenantId, identifier, claims)));
        };
    }

    public static HandleByIdDelegate<TEntity> CreateByIdHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, endpointFactory,
            repository, user, identifier, id) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            
            var entry = await repository.ByIdAsync(tenantId, identifier, claims, id);

            if (entry == null)
            {
                return Results.NotFound();
            }

            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .WithTenantAndEntityRightCheck(RightView, entry)
                .ExecuteAsync(() => Task.FromResult(Results.Ok(entry)));
        };
    }
    
    public static HandleSaveDelegate<TEntity> CreateSaveHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, endpointFactory,
            repository, user, identifier, value) =>
        {
            var currentUserId = principalUtils.GetUserId(user);
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            
            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .WithTenantAndEntityRightCheck(
                    identifier == DefaultQuery ? RightEdit : identifier, value)
                .ExecuteAsync(async () =>
                {
                    await repository.SaveAsync(tenantId, currentUserId, identifier, claims, value);

                    return Results.Ok();
                });
        };
    }
    
    public static HandleRemoveDelegate<TEntity> CreateRemoveHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, endpointFactory,
            repository, user, id) =>
        {
            var currentUserId = principalUtils.GetUserId(user);
            var tenantId = principalUtils.GetUserTenandId(user);
            var claims = principalUtils.GetUserClaims(user);

            var entry = await repository.ByIdAsync(tenantId, DefaultQuery, claims, id);

            if (entry == null)
            {
                return Results.NotFound();
            }
            
            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .WithTenantAndEntityRightCheck(
                    RightDelete, entry)
                .ExecuteAsync(async () =>
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
                });
        };
    }
    
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
    public static HandleImportDelegate CreateImportHandler(string application, string entity)
    {
        return async (schedulerFactory, principalUtils, endpointFactory, jobMetaRepository, user, storageAdapter, identifier, files) =>
        {
            var currentUserId = principalUtils.GetUserId(user);
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            
            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .ExecuteAsync(async () =>
                {
                    foreach (var file in files)
                    {
                        var jobData = new JobDataMap();

                        jobData["tenantId"] = tenantId;
                        jobData["userId"] = currentUserId;
                        jobData["identifier"] = identifier;
                        jobData["claims"] = JsonSerializer.Serialize(claims);
                        jobData["filename"] = file.FileName;

                        await storageAdapter.UploadFileForOwnerAsync(currentUserId.ToString(), file.FileName, file.ContentType, file.OpenReadStream());

                        var job = await jobMetaRepository.CreateJobAsync(tenantId, currentUserId, "meta",
                            "import", JsonSerializer.Serialize(jobData));

                        jobData["jobId"] = job.Id;

                        await (await schedulerFactory.GetScheduler()).TriggerJob(JobKey.Create("import", entity), jobData);
                    }

                    return Results.Created();
                });
        };        
    }
    
    public static HandleExportDelegate<TEntity> CreateExportHandler<TEntity>(string application, string entity) where TEntity : class
    {
        return async (principalUtils, endpointFactory,
            repository, user, identifier, request) =>
        {
            var tenantId = principalUtils.GetUserTenandId(user);

            var claims = principalUtils.GetUserClaims(user);
            var query = request.Query;
            
            var queryParams = new Dictionary<string, object>();

            foreach (var queryEntry in query)
            {
                queryParams.Add(queryEntry.Key, queryEntry.Value);
            }
            
            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .WithTenantAndEntityRightCheck(identifier, queryParams)
                .ExecuteAsync(async () =>
                {
                    var export = await repository.ExportAsync(tenantId, identifier, claims, queryParams);
        
                    return Results.Content(Encoding.UTF8.GetString(export.Data), export.MediaType);
                });
        }; 
    }
    
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
    public static HandleExportUrlDelegate<TEntity> CreateExportUrlHandler<TEntity>(string application, string entity)
        where TEntity : class
    {
        return async (principalUtils, endpointFactory, exportMetaRepository, repository, storageAdapter, user, identifier, request) =>
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
            
            return await endpointFactory.Create(tenantId, application, entity)
                .WithClaims(claims)
                .WithTenantAndEntityRightCheck(identifier, queryParams)
                .ExecuteAsync(async () =>
                {
                    var export = await repository.ExportAsync(tenantId, identifier, claims, queryParams);
        
                    var exportEntry = await exportMetaRepository.NewAsync(tenantId, DefaultQuery, claims);

                    exportEntry.Entity = entity;
                    exportEntry.Query = identifier;
                    exportEntry.MediaType = export.MediaType;
                    exportEntry.ExpirationStamp = DateTime.Now.AddDays(1);

                    await storageAdapter.UploadFileForOwnerAsync("export", $"{exportEntry.Id}{MimeTypeMap.GetExtension(export.MediaType)}", export.MediaType, new MemoryStream(export.Data));
        
                    await exportMetaRepository.SaveAsync(tenantId, currentUserId, DefaultQuery, claims, exportEntry);

                    return Results.Content(exportEntry.Id.ToString());
                });
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
            else if (queryEntry.Value.Count == 1)
            {
                queryParams.Add(queryEntry.Key, queryEntry.Value.ToString());
            }
        }

        return queryParams;
    }
}