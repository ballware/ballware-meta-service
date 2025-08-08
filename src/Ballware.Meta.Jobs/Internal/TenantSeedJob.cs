using Ballware.Meta.Data;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Newtonsoft.Json;
using Quartz;

namespace Ballware.Meta.Jobs.Internal;

class TenantSeedJob : IJob
{
    public static readonly JobKey Key = new JobKey("seed", "tenant");

    private IMetadataSeeder Seeder { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }
    private IJobMetaRepository JobMetaRepository { get; }
    
    public TenantSeedJob(IMetadataSeeder seeder, 
        ITenantMetaRepository tenantMetaRepository,
        IJobMetaRepository jobMetaRepository)
    {
        Seeder = seeder;
        TenantMetaRepository = tenantMetaRepository;
        JobMetaRepository = jobMetaRepository;       
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetGuidValue("tenantId");
        var jobId = context.MergedJobDataMap.GetGuidValue("jobId");
        var userId = context.MergedJobDataMap.GetGuidValue("userId");

        try
        {
            await JobMetaRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.InProgress, string.Empty);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);

            if (await Seeder.GetAdminTenantIdAsync() == tenantId)
            {
                await Seeder.SeedAdminTenantAsync(tenant);
            }
            else if (tenant != null)
            {
                await Seeder.SeedCustomerTenantAsync(tenant);
            }

            await JobMetaRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.Finished, string.Empty);

        }
        catch (Exception ex)
        { 
            await JobMetaRepository.UpdateJobAsync(tenantId, userId, jobId, JobStates.Error, JsonConvert.SerializeObject(ex));    
            
            // do you want the job to refire?
            throw new JobExecutionException(msg: ex.Message, refireImmediately: false, cause: ex);
        }
        
    }
}