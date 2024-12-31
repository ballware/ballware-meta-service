using Ballware.Meta.Tenant.Data.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Tenant.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareTenantStorage(this IServiceCollection services,
        Action<TenantStorageBuilder>? configureOptions = null)
    {
        var defaultStorageProviderRegistry = new DefaultStorageProviderRegistry();
        
        services.AddSingleton<IStorageProviderRegistry>(defaultStorageProviderRegistry);
        services.AddSingleton<ITenantStorageProvider, TenantStorageProviderProxy>();

        if (configureOptions != null)
        {
            configureOptions(new TenantStorageBuilder(services, defaultStorageProviderRegistry));
        }
        
        return services;
    }
}