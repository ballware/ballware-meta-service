using AutoMapper;
using Ballware.Meta.Service.Dtos;

namespace Ballware.Meta.Service.Mappings;

public class ServiceApiProfile : Profile
{
    public ServiceApiProfile()
    {
        CreateMap<Data.Public.EntityMetadata, ServiceEntityDto>();
        CreateMap<Data.Public.Tenant, ServiceTenantDto>();
    }
}