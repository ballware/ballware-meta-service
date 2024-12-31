namespace Ballware.Meta.Data.Repository;

public interface ISubscriptionMetaRepository
{
    Task<Subscription?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id);

    Task<IEnumerable<Subscription>> GetActiveSubscriptionsByFrequencyAsync(int frequency);

    Task SetLastErrorAsync(Guid tenantId, Guid id, string message);
}