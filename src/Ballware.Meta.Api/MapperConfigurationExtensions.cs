using AutoMapper;
using Ballware.Meta.Api.Mappings;

namespace Ballware.Meta.Api;

public static class MapperConfigurationExtensions
{
    public static IMapperConfigurationExpression AddBallwareMetaApiMappings(
        this IMapperConfigurationExpression configuration)
    {
        configuration.AddProfile<MetaApiProfile>();
        configuration.AddProfile<ServiceApiProfile>();

        return configuration;
    }
}