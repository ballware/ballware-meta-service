using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Repository;
using Ballware.Generic.Schema.Client;
using Newtonsoft.Json;
using Quartz;

namespace Ballware.Meta.Service.Extensions;

public class GenericSchemaTenantRepositoryHook 
    : IRepositoryHook<Ballware.Meta.Data.Public.Tenant, Ballware.Meta.Data.Persistables.Tenant>
{
    private ILogger<GenericSchemaTenantRepositoryHook> Logger { get; }
    private ISchedulerFactory SchedulerFactory { get; }
    private IJobMetaRepository JobMetaRepository { get; }
    private GenericSchemaClient SchemaClient { get; }
    
    public GenericSchemaTenantRepositoryHook(ILogger<GenericSchemaTenantRepositoryHook> logger, ISchedulerFactory schedulerFactory, IJobMetaRepository jobMetaRepository, GenericSchemaClient schemaClient)
    {
        Logger = logger;
        SchedulerFactory = schedulerFactory;
        JobMetaRepository = jobMetaRepository;
        SchemaClient = schemaClient;
    }
    
    public void AfterSave(Guid? userId, string identifier, IDictionary<string, object> claims, Ballware.Meta.Data.Public.Tenant value,
        Ballware.Meta.Data.Persistables.Tenant persistable, bool insert)
    {
        if (value.ManagedDatabase && !string.IsNullOrEmpty(value.ProviderModelDefinition))
        {
            SchemaClient.TenantCreateOrUpdateSchemaForTenant(value.Id, new TenantSchema()
            {
                Provider = value.Provider,
                UserId = userId.Value,
                SerializedTenantModel = value.ProviderModelDefinition
            });    
        }
        
        if (value.Seed)
        {
            var jobData = new JobDataMap();

            jobData["tenantId"] = value.Id;
            jobData["userId"] = userId ?? Guid.Empty;

            var job = JobMetaRepository.CreateJobAsync(value.Id, userId ?? Guid.Empty, "tenant",
                "seed", JsonConvert.SerializeObject(jobData)).GetAwaiter().GetResult();

            jobData["jobId"] = job.Id;

            SchedulerFactory.GetScheduler().GetAwaiter().GetResult().TriggerJob(JobKey.Create("seed", "tenant"), jobData).GetAwaiter().GetResult();
        }
    }

    public void BeforeRemove(Guid? userId, IDictionary<string, object> claims, Tenant persistable)
    {
        if (persistable.ManagedDatabase)
        {
            SchemaClient.TenantDropSchemaForTenant(persistable.Uuid, userId);
        }
    }
}