using AutoMapper;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Service.Dtos;
using Newtonsoft.Json;

namespace Ballware.Meta.Service.Mappings;

public class ServiceApiProfile : Profile
{
    public ServiceApiProfile()
    {
        CreateMap<EntityMetadata, ServiceEntityDto>()
            .ForMember(dst => dst.CustomScripts, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<EntityCustomScripts>(src.CustomScripts ?? "{}")))
            .ForMember(dst => dst.ListQuery, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityQueryEntryDto[]>(src.ListQuery ?? "[]")))
            .ForMember(dst => dst.NewQuery, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityQueryEntryDto[]>(src.NewQuery ?? "[]")))
            .ForMember(dst => dst.ByIdQuery, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityQueryEntryDto[]>(src.ByIdQuery ?? "[]")))
            .ForMember(dst => dst.SaveStatement, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityQueryEntryDto[]>(src.SaveStatement ?? "[]")))
            .ForMember(dst => dst.CustomFunctions, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityCustomFunctionDto[]>(src.CustomFunctions ?? "[]")));

        CreateMap<EntityCustomScripts, ServiceEntityCustomScriptsDto>();

        CreateMap<Data.Public.Tenant, ServiceTenantDto>();
            //.ForMember(dst => dst.Objects, opt => opt.MapFrom(src => src.Objects));
        
        CreateMap<Notification, ServiceNotificationDto>();
            
        CreateMap<Export, ServiceExportDto>();
        CreateMap<ServiceExportDto, Export>();
    }
}