namespace Ballware.Meta.Caching;

public interface ITenantAwareEntityCache
{
    TItem? GetItem<TItem>(Guid tenantId, string entity, string key) where TItem : class;
    bool TryGetItem<TItem>(Guid tenantId, string entity, string key, out TItem? item) where TItem : class;
    void SetItem<TItem>(Guid tenantId, string entity, string key, TItem value) where TItem : class;
    void PurgeItem(Guid tenantId, string entity, string key);
}