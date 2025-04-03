using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class SubscriptionMetaRepository : TenantableBaseRepository<Public.Subscription, Persistables.Subscription>, ISubscriptionMetaRepository
{
    public SubscriptionMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) { }

    public async Task<Public.Subscription?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id)
    {
        var result = await Context.Subscriptions.SingleOrDefaultAsync(c => c.TenantId == tenantId && c.Uuid == id);

        return result != null ? Mapper.Map<Public.Subscription>(result) : null;
    }

    public virtual async Task<IEnumerable<Public.Subscription>> GetActiveSubscriptionsByFrequencyAsync(int frequency)
    {
        return await Task.Run(() => Context.Subscriptions.Where(s => s.Active && s.Frequency == frequency).Select(s => Mapper.Map<Public.Subscription>(s)));
    }

    public virtual async Task SetLastErrorAsync(Guid tenantId, Guid id, string message)
    {
        var subscription = await Context.Subscriptions.SingleAsync(c => c.TenantId == tenantId && c.Uuid == id);

        subscription.LastSendStamp = DateTime.Now;
        subscription.LastError = message ?? "OK";

        Context.Update(subscription);

        await Context.SaveChangesAsync();
    }
    
    public virtual async Task<IEnumerable<SubscriptionSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Subscriptions
            .Where(p => p.TenantId == tenantId)
            .Select(d => new SubscriptionSelectListEntry { Id = d.Uuid, NotificationId = d.NotificationId, UserId = d.UserId, Active = d.Active }));
    }
    
    public virtual async Task<SubscriptionSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.Subscriptions.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(d => new SubscriptionSelectListEntry { Id = d.Uuid, NotificationId = d.NotificationId, UserId = d.UserId, Active = d.Active })
            .FirstOrDefaultAsync();
    }
}