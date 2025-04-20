using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Ballware.Storage.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;

namespace Ballware.Meta.Service.Jobs;

public class MetaImportJob<TEntity, TRepository> 
    : IJob where TEntity : class where TRepository : ITenantableRepository<TEntity>
{
    private IServiceProvider ServiceProvider { get; }
    private IJobMetaRepository JobRepository { get; }
    private ITenantMetaRepository TenantRepository { get; }
    private ITenantRightsChecker TenantRightsChecker { get; }
    private BallwareStorageClient StorageClient { get; }
    
    public MetaImportJob(IServiceProvider serviceProvider, IJobMetaRepository jobRepository, ITenantMetaRepository tenantRepository, ITenantRightsChecker tenantRightsChecker, BallwareStorageClient storageClient)
    {
        ServiceProvider = serviceProvider;
        JobRepository = jobRepository;
        TenantRepository = tenantRepository;
        TenantRightsChecker = tenantRightsChecker;
        StorageClient = storageClient;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var jobKey = context.Trigger.JobKey;
        var tenantId = context.MergedJobDataMap.GetGuidValue("tenantId");
        var jobId = context.MergedJobDataMap.GetGuidValue("jobId");
        var userId = context.MergedJobDataMap.GetGuidValue("userId");
        var identifier = context.MergedJobDataMap.GetString("identifier");
        var claims = JsonConvert.DeserializeObject<Dictionary<string, object>>(context.MergedJobDataMap.GetString("claims"));
        var filename = context.MergedJobDataMap.GetString("filename");
        
        var tenant = await TenantRepository.ByIdAsync(tenantId);
        var repository = ServiceProvider.GetRequiredService<TRepository>();
        
        try
        {
            if (tenant == null)
            {
                throw new ArgumentException($"Tenant {tenantId} unknown");
            }
            
            if (identifier == null) 
            {
                throw new ArgumentException($"Identifier unknown");
            }
            
            await JobRepository.UpdateJobAsync(tenant, userId, jobId, JobStates.InProgress, string.Empty);
            
            var file = await StorageClient.FileByNameForOwnerAsync(userId.ToString(), filename);

            await repository.ImportAsync(tenantId, userId, identifier, claims, file.Stream, async (item) =>
            {
                var tenantAuthorized = await TenantRightsChecker.HasRightAsync(tenant, "meta", jobKey.Group, claims, identifier);

                return tenantAuthorized;
            });

            await StorageClient.RemoveFileForOwnerAsync(userId.ToString(), filename);
            await JobRepository.UpdateJobAsync(tenant, userId, jobId, JobStates.Finished, string.Empty);
        }
        catch (Exception ex)
        {
            if (tenant != null)
            {
                await JobRepository.UpdateJobAsync(tenant, userId, jobId, JobStates.Error, JsonConvert.SerializeObject(ex));    
            }
            
            // do you want the job to refire?
            throw new JobExecutionException(msg: "", refireImmediately: false, cause: ex);
        }
    }
}