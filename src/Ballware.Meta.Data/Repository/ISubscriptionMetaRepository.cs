namespace Ballware.Meta.Data.Repository;

public interface ISubscriptionMetaRepository : ITenantableRepository<Public.Subscription>
{
    Task<Public.Subscription?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id);

    Task<IEnumerable<Public.Subscription>> GetActiveSubscriptionsByFrequencyAsync(int frequency);

    Task SetLastErrorAsync(Guid tenantId, Guid id, string message);
}