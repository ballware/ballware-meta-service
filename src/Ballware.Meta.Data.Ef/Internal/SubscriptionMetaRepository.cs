using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class SubscriptionMetaRepository : ISubscriptionMetaRepository
{
    private MetaDbContext DbContext { get; }
    
    public SubscriptionMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public async Task<Subscription?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id)
    {
        return await DbContext.Subscriptions.SingleOrDefaultAsync(c => c.TenantId == tenantId && c.Uuid == id);
    }

    public virtual async Task<IEnumerable<Subscription>> GetActiveSubscriptionsByFrequencyAsync(int frequency)
    {
        return await Task.FromResult(DbContext.Subscriptions.Where(s => s.Active && s.Frequency == frequency));
    }

    public virtual async Task SetLastErrorAsync(Guid tenantId, Guid id, string message)
    {
        var subscription = await DbContext.Subscriptions.SingleAsync(c => c.TenantId == tenantId && c.Uuid == id);
        
        subscription.LastSendStamp = DateTime.Now;
        subscription.LastError = message ?? "OK";
        
        DbContext.Update(subscription);
        await DbContext.SaveChangesAsync();
    }
}