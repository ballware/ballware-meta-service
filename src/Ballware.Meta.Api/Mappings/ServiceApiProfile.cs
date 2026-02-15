using Mapster;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Data.Public;
using Newtonsoft.Json;

namespace Ballware.Meta.Api.Mappings;

public class ServiceApiProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<EntityMetadata, ServiceEntity>()
            .Map(dst => dst.CustomScripts, src => JsonConvert.DeserializeObject<EntityCustomScripts>(src.CustomScripts ?? "{}"))
            .Map(dst => dst.ListQuery, src => JsonConvert.DeserializeObject<ServiceEntityQueryEntry[]>(src.ListQuery ?? "[]"))
            .Map(dst => dst.NewQuery, src => JsonConvert.DeserializeObject<ServiceEntityQueryEntry[]>(src.NewQuery ?? "[]"))
            .Map(dst => dst.ByIdQuery, src => JsonConvert.DeserializeObject<ServiceEntityQueryEntry[]>(src.ByIdQuery ?? "[]"))
            .Map(dst => dst.SaveStatement, src => JsonConvert.DeserializeObject<ServiceEntityQueryEntry[]>(src.SaveStatement ?? "[]"))
            .Map(dst => dst.CustomFunctions, src => JsonConvert.DeserializeObject<ServiceEntityCustomFunction[]>(src.CustomFunctions ?? "[]"));

        config.NewConfig<EntityCustomScripts, ServiceEntityCustomScripts>();
        config.NewConfig<Tenant, ServiceTenant>()
            .Map(dst => dst.ReportDatasourceDefinitions, src => JsonConvert.DeserializeObject<IEnumerable<ServiceTenantReportDatasourceDefinition>>(src.ReportSchemaDefinition ?? "[]"));

        config.NewConfig<ReportDatasourceDefinition, ServiceTenantReportDatasourceDefinition>();
        config.NewConfig<ReportDatasourceTable, ServiceTenantReportDatasourceTable>();
    }
}