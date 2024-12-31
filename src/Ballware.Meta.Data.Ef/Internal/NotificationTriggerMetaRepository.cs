using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class NotificationTriggerMetaRepository : INotificationTriggerMetaRepository
{
    private MetaDbContext DbContext { get; }

    public NotificationTriggerMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<NotificationTrigger> NewAsync(Guid tenantId, Guid notificationId, string notificationParams, Guid? userId)
    {
        return await Task.FromResult(new NotificationTrigger()
        {
            TenantId = tenantId,
            Uuid = Guid.NewGuid(),
            CreatorId = userId,
            CreateStamp = DateTime.Now,
            NotificationId = notificationId,
            Params = notificationParams
        });
    }

    public async Task SaveAsync(Guid tenantId, NotificationTrigger notificationTrigger, Guid? userId)
    {
        var existing = await DbContext.NotificationTriggers.SingleOrDefaultAsync(t => t.TenantId == tenantId && t.Uuid == notificationTrigger.Uuid);

        if (existing == null)
        {
            notificationTrigger.TenantId = tenantId;
            notificationTrigger.CreatorId = userId;
            notificationTrigger.CreateStamp = DateTime.Now;
            
            existing = DbContext.NotificationTriggers.Add(notificationTrigger).Entity;
        }
        
        existing.NotificationId = notificationTrigger.NotificationId;
        existing.Params = notificationTrigger.Params;
        existing.Finished = notificationTrigger.Finished;
        existing.LastChangerId = userId;
        existing.LastChangeStamp = DateTime.Now;
        
        await DbContext.SaveChangesAsync();
    }
}
