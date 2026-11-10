using Mapster;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Api.Mappings;

public class MetaApiProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<EntityMetadata, MetaEntity>();
        config.NewConfig<Tenant, MetaTenant>();
    }
}