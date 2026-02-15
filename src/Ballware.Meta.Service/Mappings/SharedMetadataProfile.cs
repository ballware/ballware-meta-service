using Ballware.Meta.Data.Common;
using Mapster;

namespace Ballware.Meta.Service.Mappings;

public class SharedMetadataProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Ballware.Shared.Api.Public.JobStates, JobStates>();
        config.NewConfig<JobStates, Ballware.Shared.Api.Public.JobStates>();
    }
}