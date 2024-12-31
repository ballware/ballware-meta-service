using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Tenant.Data;

public class TenantStorageBuilder
{
    public IServiceCollection Services { get; }
    public IStorageProviderRegistry ProviderRegistry { get; }

    internal TenantStorageBuilder(IServiceCollection services, IStorageProviderRegistry storageProviderRegistry)
    {
        Services = services;
        ProviderRegistry = storageProviderRegistry;
    }
}