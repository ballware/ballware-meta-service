using Ballware.Meta.Api.Mappings;
using Mapster;

namespace Ballware.Meta.Api;

public static class MapperConfigurationExtensions
{
    public static TypeAdapterConfig AddBallwareMetaApiMappings(
        this TypeAdapterConfig configuration)
    {
        new MetaApiProfile().Register(configuration);
        new ServiceApiProfile().Register(configuration);

        return configuration;
    }
}