using AutoMapper;
using Ballware.Meta.Data.Ef.Internal;
using Ballware.Meta.Data.Ef.Mapping;

namespace Ballware.Meta.Data.Ef;

public static class MapperConfigurationExtensions
{
    public static IMapperConfigurationExpression AddBallwareStorageMappings(
        this IMapperConfigurationExpression configuration)
    {
        configuration.AddProfile<StorageMappingProfile>();

        return configuration;
    }
}