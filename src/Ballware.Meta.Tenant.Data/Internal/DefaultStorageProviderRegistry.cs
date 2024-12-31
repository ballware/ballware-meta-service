namespace Ballware.Meta.Tenant.Data.Internal;

class DefaultStorageProviderRegistry : IStorageProviderRegistry
{
    private readonly Dictionary<string, ITenantStorageProvider> _storageProviders = new();
    
    public void RegisterStorageProvider(string providerName, ITenantStorageProvider storageProvider)
    {
        lock (_storageProviders)
        {
            _storageProviders[providerName] = storageProvider;
        }
    }

    public ITenantStorageProvider GetStorageProvider(string providerName)
    {
        lock (_storageProviders)
        {
            if (_storageProviders.ContainsKey(providerName))
            {
                return _storageProviders[providerName];
            }
        }

        throw new KeyNotFoundException($"No storage provider found for provider {providerName}");
    }
}