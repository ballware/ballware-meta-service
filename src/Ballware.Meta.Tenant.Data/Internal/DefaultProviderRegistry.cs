namespace Ballware.Meta.Tenant.Data.Internal;

class DefaultProviderRegistry : IProviderRegistry
{
    private readonly Dictionary<string, ITenantStorageProvider> _storageProviders = new();
    private readonly Dictionary<string, ITenantLookupProvider> _lookupProviders = new();

    public void RegisterStorageProvider(string providerName, ITenantStorageProvider provider)
    {
        lock (_storageProviders)
        {
            _storageProviders[providerName] = provider;
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

    public void RegisterLookupProvider(string providerName, ITenantLookupProvider provider)
    {
        lock (_lookupProviders)
        {
            _lookupProviders[providerName] = provider;
        }
    }

    public ITenantLookupProvider GetLookupProvider(string providerName)
    {
        lock (_lookupProviders)
        {
            if (_lookupProviders.ContainsKey(providerName))
            {
                return _lookupProviders[providerName];
            }
        }

        throw new KeyNotFoundException($"No lookup provider found for provider {providerName}");
    }
}