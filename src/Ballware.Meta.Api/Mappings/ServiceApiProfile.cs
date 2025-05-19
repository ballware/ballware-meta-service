using System.Text.Json;
using AutoMapper;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Api.Mappings;

public class ServiceApiProfile : Profile
{
    public ServiceApiProfile()
    {
        CreateMap<EntityMetadata, ServiceEntity>()
            .ForMember(dst => dst.CustomScripts, opt => opt.MapFrom(src => JsonSerializer.Deserialize<EntityCustomScripts>(src.CustomScripts ?? "{}", JsonSerializerOptions.Default)))
            .ForMember(dst => dst.ListQuery, opt => opt.MapFrom(src => JsonSerializer.Deserialize<ServiceEntityQueryEntry[]>(src.ListQuery ?? "[]", JsonSerializerOptions.Default)))
            .ForMember(dst => dst.NewQuery, opt => opt.MapFrom(src => JsonSerializer.Deserialize<ServiceEntityQueryEntry[]>(src.NewQuery ?? "[]", JsonSerializerOptions.Default)))
            .ForMember(dst => dst.ByIdQuery, opt => opt.MapFrom(src => JsonSerializer.Deserialize<ServiceEntityQueryEntry[]>(src.ByIdQuery ?? "[]", JsonSerializerOptions.Default)))
            .ForMember(dst => dst.SaveStatement, opt => opt.MapFrom(src => JsonSerializer.Deserialize<ServiceEntityQueryEntry[]>(src.SaveStatement ?? "[]", JsonSerializerOptions.Default)))
            .ForMember(dst => dst.CustomFunctions, opt => opt.MapFrom(src => JsonSerializer.Deserialize<ServiceEntityCustomFunction[]>(src.CustomFunctions ?? "[]", JsonSerializerOptions.Default)));

        CreateMap<EntityCustomScripts, ServiceEntityCustomScripts>();

        CreateMap<Tenant, ServiceTenant>()
            .ForMember(dst => dst.ReportDatasourceDefinitions, opt => opt.MapFrom(src => JsonSerializer.Deserialize<IEnumerable<ServiceTenantReportDatasourceDefinition>>(src.ReportSchemaDefinition ?? "[]", JsonSerializerOptions.Default)));
        
        CreateMap<Notification, ServiceNotification>();
            
        CreateMap<Export, ServiceExport>();
        CreateMap<ServiceExport, Export>();
    }
}