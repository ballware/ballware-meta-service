using Ballware.Meta.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Ballware.Meta.Service.Cache;

class InMemoryEntityCache : ITenantAwareEntityCache
{
    private ILogger<InMemoryEntityCache> Logger { get; }
    private IMemoryCache Cache { get; }

    private string BuildKey(Guid tenantId, string entity, string key)
    {
        return $"{tenantId}_{entity}_{key}".ToLowerInvariant();
    }

    public InMemoryEntityCache(ILogger<InMemoryEntityCache> logger, IMemoryCache memoryCache)
    {
        Logger = logger;
        Cache = memoryCache;
    }

    public TItem? GetItem<TItem>(Guid tenantId, string entity, string key)
    {
        Cache.TryGetValue<TItem>(BuildKey(tenantId, entity, key), out TItem? item);

        if (item != null)
        {
            Logger.LogDebug("Cache hit for {BuildKey}", BuildKey(tenantId, entity, key));
        }
        else
        {
            Logger.LogDebug("Cache fail for {BuildKey}", BuildKey(tenantId, entity, key));
        }

        return item;
    }

    public bool TryGetItem<TItem>(Guid tenantId, string entity, string key, out TItem? item)
    {
        item = GetItem<TItem>(tenantId, entity, key);

        return item != null;
    }

    public void SetItem<TItem>(Guid tenantId, string entity, string key, TItem value)
    {
        Cache.Set(BuildKey(tenantId, entity, key), value);

        Logger.LogDebug("Cache update for {BuildKey}", BuildKey(tenantId, entity, key));
    }

    public void PurgeItem(Guid tenantId, string entity, string key)
    {
        Cache.Remove(BuildKey(tenantId, entity, key));

        Logger.LogDebug("Cache purge for {BuildKey}", BuildKey(tenantId, entity, key));
    }
}