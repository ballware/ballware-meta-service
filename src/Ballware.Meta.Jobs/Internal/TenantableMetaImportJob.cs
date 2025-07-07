using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;

namespace Ballware.Meta.Jobs.Internal;

public class TenantableMetaImportJob<TEntity, TRepository> 
    : IJob where TEntity : class where TRepository : ITenantableRepository<TEntity>
{
    private IServiceProvider ServiceProvider { get; }
    private IJobMetaRepository JobRepository { get; }
    private ITenantMetaRepository TenantRepository { get; }
    private ITenantRightsChecker TenantRightsChecker { get; }
    private IJobsFileStorageAdapter StorageAdapter { get; }
    
    public TenantableMetaImportJob(IServiceProvider serviceProvider, IJobMetaRepository jobRepository, ITenantMetaRepository tenantRepository, ITenantRightsChecker tenantRightsChecker, IJobsFileStorageAdapter storageAdapter)
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
        context.MergedJobDataMap.TryGetString("identifier", out var identifier) ;
                         
        var claims = Utils.DropNullMember(Utils.NormalizeJsonMember(JsonConvert.DeserializeObject<Dictionary<string, object?>>(context.MergedJobDataMap.GetString("claims") ?? "{}")
                                                                    ?? new Dictionary<string, object?>()));
        context.MergedJobDataMap.TryGetGuidValue("file", out var temporaryId);
        
        var tenant = await TenantRepository.ByIdAsync(tenantId);
        var repository = ServiceProvider.GetRequiredService<TRepository>();
        
        try
        {
            if (identifier == null) 
            {
                throw new ArgumentException($"Identifier undefined");
            }

            if (temporaryId == Guid.Empty)
            {
                throw new ArgumentException($"File undefined");
            }
            
            if (tenant == null)
            {
                throw new ArgumentException($"Tenant {tenantId} unknown");
            }
            
            await JobRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.InProgress, string.Empty);
            
            var file = await StorageAdapter.TemporaryFileByIdAsync(tenantId, temporaryId);

            await repository.ImportAsync(tenantId, userId, identifier, claims, file, async (item) =>
            {
                var tenantAuthorized = await TenantRightsChecker.HasRightAsync(tenant, "meta", jobKey.Group, claims, identifier);

                return tenantAuthorized;
            });

            await StorageAdapter.RemoveTemporaryFileByIdBehalfOfUserAsync(tenantId, userId, temporaryId);
            await JobRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.Finished, string.Empty);
        }
        catch (Exception ex)
        {
            if (tenant != null)
            {
                await JobRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.Error, JsonConvert.SerializeObject(ex));    
            }
            
            // do you want the job to refire?
            throw new JobExecutionException(msg: ex.Message, refireImmediately: false, cause: ex);
        }
    }
}