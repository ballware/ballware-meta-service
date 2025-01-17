using Ballware.Meta.Tenant.Data.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Tenant.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareTenantStorage(this IServiceCollection services,
        Action<TenantStorageBuilder>? configureOptions = null)
    {
        var defaultStorageProviderRegistry = new DefaultProviderRegistry();

        services.AddSingleton<IProviderRegistry>(defaultStorageProviderRegistry);
        services.AddSingleton<ITenantStorageProvider, TenantStorageProviderProxy>();
        services.AddSingleton<ITenantLookupProvider, TenantLookupProviderProxy>();

        if (configureOptions != null)
        {
            configureOptions(new TenantStorageBuilder(services, defaultStorageProviderRegistry));
        }

        return services;
    }
}