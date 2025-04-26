using AutoMapper;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Data.Public;
using Newtonsoft.Json;

namespace Ballware.Meta.Api.Mappings;

public class ServiceApiProfile : Profile
{
    public ServiceApiProfile()
    {
        CreateMap<EntityMetadata, ServiceEntity>()
            .ForMember(dst => dst.CustomScripts, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<EntityCustomScripts>(src.CustomScripts ?? "{}")))
            .ForMember(dst => dst.ListQuery, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityQueryEntry[]>(src.ListQuery ?? "[]")))
            .ForMember(dst => dst.NewQuery, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityQueryEntry[]>(src.NewQuery ?? "[]")))
            .ForMember(dst => dst.ByIdQuery, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityQueryEntry[]>(src.ByIdQuery ?? "[]")))
            .ForMember(dst => dst.SaveStatement, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityQueryEntry[]>(src.SaveStatement ?? "[]")))
            .ForMember(dst => dst.CustomFunctions, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<ServiceEntityCustomFunction[]>(src.CustomFunctions ?? "[]")));

        CreateMap<EntityCustomScripts, ServiceEntityCustomScripts>();

        CreateMap<Tenant, ServiceTenant>()
            .ForMember(dst => dst.ReportDatasourceDefinitions, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<IEnumerable<ServiceTenantReportDatasourceDefinition>>(src.ReportSchemaDefinition ?? "[]")));
        
        CreateMap<Notification, ServiceNotification>();
            
        CreateMap<Export, ServiceExport>();
        CreateMap<ServiceExport, Export>();
    }
}