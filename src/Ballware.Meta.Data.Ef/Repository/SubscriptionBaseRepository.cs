using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Repository;

public abstract class SubscriptionBaseRepository : TenantableRepository<Public.Subscription, Persistables.Subscription>, ISubscriptionMetaRepository
{
    private IMetaDbContext MetaContext { get; }

    public SubscriptionBaseRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Subscription, Persistables.Subscription>? hook = null)
        : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }

    public async Task<Public.Subscription?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id)
    {
        var result = await MetaContext.Subscriptions.SingleOrDefaultAsync(c => c.TenantId == tenantId && c.Uuid == id);

        return result != null ? Mapper.Map<Public.Subscription>(result) : null;
    }

    public virtual async Task<IEnumerable<Public.Subscription>> GetActiveSubscriptionsByTenantAndFrequencyAsync(Guid tenantId, int frequency)
    {
        return await Task.Run(() => MetaContext.Subscriptions.Where(s => s.TenantId == tenantId && s.Active && s.Frequency == frequency).Select(s => Mapper.Map<Public.Subscription>(s)));
    }

    public virtual async Task SetLastErrorAsync(Guid tenantId, Guid id, string message)
    {
        var subscription = await MetaContext.Subscriptions.SingleAsync(c => c.TenantId == tenantId && c.Uuid == id);

        subscription.LastSendStamp = DateTime.Now;
        subscription.LastError = message ?? "OK";

        MetaContext.Subscriptions.Update(subscription);

        await Context.SaveChangesAsync();
    }
    
    public virtual async Task<IEnumerable<SubscriptionSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(MetaContext.Subscriptions
            .Where(p => p.TenantId == tenantId)
            .Select(d => new SubscriptionSelectListEntry { Id = d.Uuid, NotificationId = d.NotificationId, UserId = d.UserId, Active = d.Active }));
    }
    
    public virtual async Task<SubscriptionSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await MetaContext.Subscriptions.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(d => new SubscriptionSelectListEntry { Id = d.Uuid, NotificationId = d.NotificationId, UserId = d.UserId, Active = d.Active })
            .FirstOrDefaultAsync();
    }

    public abstract Task<string> GenerateListQueryAsync(Guid tenantId);
}