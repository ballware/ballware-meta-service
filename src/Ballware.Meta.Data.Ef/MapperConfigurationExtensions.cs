using Ballware.Meta.Data.Ef.Mapping;
using Mapster;

namespace Ballware.Meta.Data.Ef;

public static class MapperConfigurationExtensions
{
    public static TypeAdapterConfig AddBallwareStorageMappings(
        this TypeAdapterConfig configuration)
    {
        new StorageMappingProfile().Register(configuration);
        
        return configuration;
    }
}