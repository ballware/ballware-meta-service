using Ballware.Meta.Caching;
using Ballware.Meta.Service.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ballware.Meta.Service.Cache;

class DistributedTenantAwareCache : ITenantAwareEntityCache
{
    private ILogger<DistributedTenantAwareCache> Logger { get; }
    private IDistributedCache Cache { get; }
    private CacheOptions Options { get; }

    private static string BuildKey(Guid tenantId, string entity, string key)
    {
        return $"{tenantId}_{entity}_{key}".ToLowerInvariant();
    }

    public DistributedTenantAwareCache(ILogger<DistributedTenantAwareCache> logger, IDistributedCache cache, IOptions<CacheOptions> options)
    {
        Logger = logger;
        Cache = cache;
        Options = options.Value;
    }

    public TItem? GetItem<TItem>(Guid tenantId, string entity, string key) where TItem : class
    {
        var cachedSerializedItem = Cache.GetString(BuildKey(tenantId, entity, key));
        
        if (cachedSerializedItem != null)
        {
            Logger.LogInformation("Cache hit for {BuildKey}", BuildKey(tenantId, entity, key));
            return JsonConvert.DeserializeObject<TItem>(cachedSerializedItem);
        }
        
        Logger.LogDebug("Cache fail for {BuildKey}", BuildKey(tenantId, entity, key));
        
        return null;
    }

    public bool TryGetItem<TItem>(Guid tenantId, string entity, string key, out TItem? item) where TItem : class
    {
        item = GetItem<TItem>(tenantId, entity, key);

        return item != null;
    }

    public void SetItem<TItem>(Guid tenantId, string entity, string key, TItem value) where TItem : class
    {
        Cache.SetString(BuildKey(tenantId, entity, key), JsonConvert.SerializeObject(value),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(Options.CacheExpirationHours)
            });
        
        Logger.LogInformation("Cache update for {BuildKey}", BuildKey(tenantId, entity, key));
    }

    public void PurgeItem(Guid tenantId, string entity, string key)
    {
        Cache.Remove(BuildKey(tenantId, entity, key));

        Logger.LogInformation("Cache purge for {BuildKey}", BuildKey(tenantId, entity, key));
    }
}