namespace Ballware.Meta.Caching;

public interface ITenantAwareEntityCache
{
    TItem? GetItem<TItem>(Guid tenantId, string entity, string key);
    bool TryGetItem<TItem>(Guid tenantId, string entity, string key, out TItem? item);
    void SetItem<TItem>(Guid tenantId, string entity, string key, TItem value);
    void PurgeItem(Guid tenantId, string entity, string key);
}