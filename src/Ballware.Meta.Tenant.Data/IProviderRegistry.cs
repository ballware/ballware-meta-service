namespace Ballware.Meta.Tenant.Data;

public interface IProviderRegistry
{
    void RegisterStorageProvider(string providerName, ITenantStorageProvider provider);
    ITenantStorageProvider GetStorageProvider(string providerName);

    void RegisterLookupProvider(string providerName, ITenantLookupProvider provider);
    ITenantLookupProvider GetLookupProvider(string providerName);
}