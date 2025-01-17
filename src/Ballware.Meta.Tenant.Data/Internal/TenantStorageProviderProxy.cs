using System.Data;

namespace Ballware.Meta.Tenant.Data.Internal;

class TenantStorageProviderProxy : ITenantStorageProvider
{
    private IStorageProviderRegistry StorageProviderRegistry { get; }
    
    public TenantStorageProviderProxy(IStorageProviderRegistry providerRegistry)
    {
        StorageProviderRegistry = providerRegistry;
    }

    public string GetConnectionString(Meta.Data.Public.Tenant tenant)
    {
        var provider = StorageProviderRegistry.GetStorageProvider(tenant.Provider ?? "mssql");
        
        return provider.GetConnectionString(tenant);
    }

    public IDbConnection OpenConnection(Meta.Data.Public.Tenant tenant)
    {
        var provider = StorageProviderRegistry.GetStorageProvider(tenant.Provider ?? "mssql");
        
        return provider.OpenConnection(tenant);
    }

    public async Task<IDbConnection> OpenConnectionAsync(Meta.Data.Public.Tenant tenant)
    {
        var provider = StorageProviderRegistry.GetStorageProvider(tenant.Provider ?? "mssql");
        
        return await provider.OpenConnectionAsync(tenant);
    }

    public string ApplyTenantPlaceholder(Meta.Data.Public.Tenant tenant, string source, TenantPlaceholderOptions options)
    {
        var provider = StorageProviderRegistry.GetStorageProvider(tenant.Provider ?? "mssql");

        return provider.ApplyTenantPlaceholder(tenant, source, options);
    }

    public async Task<string> ApplyTenantPlaceholderAsync(Meta.Data.Public.Tenant tenant, string source, TenantPlaceholderOptions options)
    {
        var provider = StorageProviderRegistry.GetStorageProvider(tenant.Provider ?? "mssql");
        
        return await provider.ApplyTenantPlaceholderAsync(tenant, source, options);
    }
}