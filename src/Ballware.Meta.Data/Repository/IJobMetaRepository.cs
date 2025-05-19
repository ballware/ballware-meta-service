using Ballware.Meta.Data.Common;

namespace Ballware.Meta.Data.Repository;

public interface IJobMetaRepository : ITenantableRepository<Public.Job>
{
    Task<IEnumerable<Public.Job>> PendingJobsForUser(Public.Tenant tenant, Guid userId);

    Task<Public.Job> CreateJobAsync(Public.Tenant tenant, Guid userId, string scheduler,
        string identifier, string options);

    Task<Public.Job> UpdateJobAsync(Public.Tenant tenant, Guid userId,
        Guid id, JobStates state, string? result);
}