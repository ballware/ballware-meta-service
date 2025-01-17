using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Tenant.Data;

public class TenantStorageBuilder
{
    public IServiceCollection Services { get; }
    public IProviderRegistry ProviderRegistry { get; }

    internal TenantStorageBuilder(IServiceCollection services, IProviderRegistry providerRegistry)
    {
        Services = services;
        ProviderRegistry = providerRegistry;
    }
}