using AutoMapper;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class JobMetaRepository : TenantableBaseRepository<Public.Job, Persistables.Job>, IJobMetaRepository
{
    public JobMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.Job, Persistables.Job>? hook = null) 
        : base(mapper, dbContext, hook) { }

    public virtual async Task<IEnumerable<Public.Job>> PendingJobsForUser(Guid tenantId, Guid userId)
    {
        return await Task.FromResult(Context.Jobs.Where(j => j.TenantId == tenantId && j.Owner == userId && j.State != JobStates.Finished)
            .Select(j => Mapper.Map<Public.Job>(j)));
    }

    public virtual async Task<Public.Job> CreateJobAsync(Guid tenantId, Guid userId, string scheduler, string identifier, string options)
    {
        var job = Context.Jobs.Add(new Persistables.Job()
        {
            TenantId = tenantId,
            Uuid = Guid.NewGuid(),
            CreatorId = userId,
            CreateStamp = DateTime.Now,
            Scheduler = scheduler,
            Identifier = identifier,
            Options = options,
            Owner = userId
        });

        await Context.SaveChangesAsync();

        return Mapper.Map<Public.Job>(job.Entity);
    }

    public virtual async Task<Public.Job> UpdateJobAsync(Guid tenantId, Guid userId,
        Guid id, JobStates state, string? result)
    {
        var job = await Context.Jobs.SingleOrDefaultAsync(j => j.TenantId == tenantId && j.Uuid == id);

        if (job == null)
        {
            throw new Exception("Job not found");
        }

        job.State = state;
        job.Result = result;

        var jobEntry = Context.Update(job);

        await Context.SaveChangesAsync();

        return Mapper.Map<Public.Job>(jobEntry.Entity);
    }
}