namespace Ballware.Meta.Data.Repository;

public interface IJobMetaRepository
{
    Task<IEnumerable<Job>> PendingJobsForUser(Tenant tenant, Guid userId);    

    Task<Job> CreateJobAsync(Tenant tenant, Guid userId, string scheduler,
        string identifier, string options);

    Task<Job> UpdateJobAsync(Tenant tenant, Guid userId,
        Guid id, JobStates state, string result);
}