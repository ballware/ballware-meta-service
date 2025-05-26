using AutoMapper;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Api.Mappings;

public class MetaApiProfile : Profile
{
    public MetaApiProfile()
    {
        CreateMap<EntityMetadata, MetaEntity>();
        
        CreateMap<Tenant, MetaTenant>();
    }
}