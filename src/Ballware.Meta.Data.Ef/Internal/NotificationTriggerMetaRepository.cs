using AutoMapper;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class NotificationTriggerMetaRepository : TenantableBaseRepository<Public.NotificationTrigger, Persistables.NotificationTrigger>, INotificationTriggerMetaRepository
{
    public NotificationTriggerMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) {}

    public async Task<Public.NotificationTrigger> NewAsync(Guid tenantId, Guid notificationId, string notificationParams, Guid? userId)
    {
        return await Task.FromResult(new Public.NotificationTrigger()
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId,
            Params = notificationParams
        });
    }
}
