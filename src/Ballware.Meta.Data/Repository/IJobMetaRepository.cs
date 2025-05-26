using Ballware.Meta.Data.Common;

namespace Ballware.Meta.Data.Repository;

public interface IJobMetaRepository : ITenantableRepository<Public.Job>
{
    Task<IEnumerable<Public.Job>> PendingJobsForUser(Guid tenantId, Guid userId);

    Task<Public.Job> CreateJobAsync(Guid tenantId, Guid userId, string scheduler,
        string identifier, string options);

    Task<Public.Job> UpdateJobAsync(Guid tenantId, Guid userId,
        Guid id, JobStates state, string? result);
}