using Ballware.Meta.Caching.Configuration;
using Ballware.Meta.Caching.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Caching;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareDistributedCaching(this IServiceCollection services)
    {
        services.AddSingleton<ITenantAwareEntityCache, DistributedTenantAwareCache>();

        return services;
    }

}