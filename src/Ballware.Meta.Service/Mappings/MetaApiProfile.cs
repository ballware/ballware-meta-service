using AutoMapper;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Service.Dtos;
using Newtonsoft.Json;

namespace Ballware.Meta.Service.Mappings;

public class MetaApiProfile : Profile
{
    public MetaApiProfile()
    {
        CreateMap<Data.Public.EntityMetadata, MetaEntityDto>();
        
        CreateMap<Data.Public.Tenant, MetaTenantDto>();
    }
}