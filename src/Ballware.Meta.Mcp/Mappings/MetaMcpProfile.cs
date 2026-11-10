using Mapster;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.SelectLists;
using Ballware.Meta.Mcp.Public;
using Newtonsoft.Json;

namespace Ballware.Meta.Mcp.Mappings;

public class MetaMcpProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tenant, TenantSummary>();
        config.NewConfig<Tenant, TenantNavigation>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Layout, src => JsonConvert.DeserializeObject<NavigationLayout>(src.Navigation ?? "{}"));
        
        config.NewConfig<PageSelectListEntry, PageSummary>();
        config.NewConfig<EntitySelectListEntry, EntitySummary>();
        config.NewConfig<LookupSelectListEntry, LookupSummary>();
        config.NewConfig<PickvalueAvailability, PickvalueAvailabilitySummary>();
        config.NewConfig<PickvalueSelectEntry, PickvalueSelectEntrySummary>();
        config.NewConfig<StatisticSelectListEntry, StatisticSummary>();
        config.NewConfig<Data.Public.Statistic, StatisticMetadata>();
        config.NewConfig<ProcessingStateSelectListEntry, ProcessingStateSummary>();
    }
}
