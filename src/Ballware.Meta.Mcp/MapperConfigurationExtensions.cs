using Ballware.Meta.Mcp.Mappings;
using Mapster;

namespace Ballware.Meta.Mcp;

public static class MapperConfigurationExtensions
{
    public static TypeAdapterConfig AddBallwareMetaMcpMappings(
        this TypeAdapterConfig configuration)
    {
        new MetaMcpProfile().Register(configuration);

        return configuration;
    }
}