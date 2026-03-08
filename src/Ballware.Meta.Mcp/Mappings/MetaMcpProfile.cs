using Mapster;
using Ballware.Meta.Data.Public;
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
    }
}