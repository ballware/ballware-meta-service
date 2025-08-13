using AutoMapper;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Repository;

public class JobBaseRepository : TenantableRepository<Public.Job, Persistables.Job>, IJobMetaRepository
{
    private IMetaDbContext MetaContext { get; }

    public JobBaseRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Job, Persistables.Job>? hook = null)
        : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }

    public virtual async Task<IEnumerable<Public.Job>> PendingJobsForUser(Guid tenantId, Guid userId)
    {
        return await Task.FromResult(MetaContext.Jobs.Where(j => j.TenantId == tenantId && j.Owner == userId && j.State != JobStates.Finished)
            .Select(j => Mapper.Map<Public.Job>(j)));
    }

    public virtual async Task<Public.Job> CreateJobAsync(Guid tenantId, Guid userId, string scheduler, string identifier, string options)
    {
        var job = MetaContext.Jobs.Add(new Persistables.Job()
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
        var job = await MetaContext.Jobs.SingleOrDefaultAsync(j => j.TenantId == tenantId && j.Uuid == id);

        if (job == null)
        {
            throw new Exception("Job not found");
        }

        job.State = state;
        job.Result = result;

        var jobEntry = MetaContext.Jobs.Update(job);

        await Context.SaveChangesAsync();

        return Mapper.Map<Public.Job>(jobEntry.Entity);
    }
}