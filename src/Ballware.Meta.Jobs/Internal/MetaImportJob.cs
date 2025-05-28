using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;

namespace Ballware.Meta.Jobs.Internal;

public class MetaImportJob<TEntity, TRepository> 
    : IJob where TEntity : class where TRepository : IRepository<TEntity>
{
    private IServiceProvider ServiceProvider { get; }
    private IJobMetaRepository JobRepository { get; }
    private ITenantMetaRepository TenantRepository { get; }
    private ITenantRightsChecker TenantRightsChecker { get; }
    private IJobsFileStorageAdapter StorageAdapter { get; }
    
    public MetaImportJob(IServiceProvider serviceProvider, IJobMetaRepository jobRepository, ITenantMetaRepository tenantRepository, ITenantRightsChecker tenantRightsChecker, IJobsFileStorageAdapter storageAdapter)
    {
        ServiceProvider = serviceProvider;
        JobRepository = jobRepository;
        TenantRepository = tenantRepository;
        TenantRightsChecker = tenantRightsChecker;
        StorageAdapter = storageAdapter;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var jobKey = context.Trigger.JobKey;
        var tenantId = context.MergedJobDataMap.GetGuidValue("tenantId");
        var jobId = context.MergedJobDataMap.GetGuidValue("jobId");
        var userId = context.MergedJobDataMap.GetGuidValue("userId");
        context.MergedJobDataMap.TryGetString("identifier", out var identifier);
        var claims = JsonConvert.DeserializeObject<Dictionary<string, object>>(context.MergedJobDataMap.GetString("claims") ?? "{}")
            ?? new Dictionary<string, object>();
        context.MergedJobDataMap.TryGetString("filename", out var filename);
        
        var tenant = await TenantRepository.ByIdAsync(tenantId);
        var repository = ServiceProvider.GetRequiredService<TRepository>();
        
        try
        {   
            if (identifier == null) 
            {
                throw new ArgumentException($"Identifier unknown");
            }
            
            if (filename == null) 
            {
                throw new ArgumentException($"Filename unknown");
            }
            
            if (tenant == null)
            {
                throw new ArgumentException($"Tenant {tenantId} unknown");
            }
            
            await JobRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.InProgress, string.Empty);
            
            var file = await StorageAdapter.FileByNameForOwnerAsync(userId.ToString(), filename);

            await repository.ImportAsync(userId, identifier, claims, file, async (item) =>
            {
                var tenantAuthorized = await TenantRightsChecker.HasRightAsync(tenant, "meta", jobKey.Group, claims, identifier);

                return tenantAuthorized;
            });

            await StorageAdapter.RemoveFileForOwnerAsync(userId.ToString(), filename);
            await JobRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.Finished, string.Empty);
        }
        catch (Exception ex)
        {
            if (tenant != null)
            {
                await JobRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.Error, JsonConvert.SerializeObject(ex));    
            }
            
            // do you want the job to refire?
            throw new JobExecutionException(msg: "", refireImmediately: false, cause: ex);
        }
    }
}