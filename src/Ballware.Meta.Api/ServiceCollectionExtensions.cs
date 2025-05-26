using Ballware.Meta.Api.Public;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareMetaApiDependencies(this IServiceCollection services)
    {
        services.AddScoped<EditingEndpointBuilderFactory>();

        return services;
    }
}