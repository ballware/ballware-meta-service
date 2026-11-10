using Ballware.Meta.Data.Repository;
using Ballware.Shared.Api;
using Ballware.Shared.Api.Public;
using MapsterMapper;

namespace Ballware.Meta.Service.Adapter;

public class JobMetadataProvider(IMapper mapper, IJobMetaRepository jobMetaRepository) : IJobMetadataProvider
{
    public async Task<Guid> CreateJobAsync(Guid tenantId, Guid userId, string scheduler, string identifier, string? options)
    {
        var job = await jobMetaRepository.CreateJobAsync(tenantId, userId, scheduler, identifier, options);
        
        return job.Id;
    }

    public async Task UpdateJobAsync(Guid tenantId, Guid userId, Guid id, JobStates state, string? result)
    {
        await jobMetaRepository.UpdateJobAsync(tenantId, userId, id, mapper.Map<Data.Common.JobStates>(state), result);
    }
}