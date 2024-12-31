using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class JobMetaRepository : IJobMetaRepository
{
    private MetaDbContext DbContext { get; }

    public JobMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<IEnumerable<Job>> PendingJobsForUser(Tenant tenant, Guid userId)
    {
        return await Task.FromResult(DbContext.Jobs.Where(j => j.TenantId == tenant.Uuid && j.Owner == userId && j.State != JobStates.Finished));
    }
    
    public virtual async Task<Job> CreateJobAsync(Tenant tenant, Guid userId, string scheduler, string identifier, string options)
    {
        var job = DbContext.Jobs.Add(new Job()
        {
            TenantId = tenant.Uuid,
            Uuid = Guid.NewGuid(),
            CreatorId = userId,
            CreateStamp = DateTime.Now,
            Scheduler = scheduler,
            Identifier = identifier,
            Options = options,
            Owner = userId
        });
        
        await DbContext.SaveChangesAsync();

        return job.Entity;
    }

    public virtual async Task<Job> UpdateJobAsync(Tenant tenant, Guid userId,
        Guid id, JobStates state, string result)
    {
        var job = await DbContext.Jobs.SingleOrDefaultAsync(j => j.TenantId == tenant.Uuid && j.Uuid == id);

        if (job == null)
        {
            throw new Exception("Job not found");
        }
        
        job.State = state;
        job.Result = result;
        
        var jobEntry = DbContext.Update(job);
        
        await DbContext.SaveChangesAsync();
        
        return jobEntry.Entity;
    }
}