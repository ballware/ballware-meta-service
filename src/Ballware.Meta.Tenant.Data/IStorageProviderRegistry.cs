namespace Ballware.Meta.Tenant.Data;

public interface IStorageProviderRegistry
{
    void RegisterStorageProvider(string providerName, ITenantStorageProvider storageProvider);
    ITenantStorageProvider GetStorageProvider(string providerName);
}