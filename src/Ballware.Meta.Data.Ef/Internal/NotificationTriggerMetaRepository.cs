using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class NotificationTriggerMetaRepository : TenantableRepository<Public.NotificationTrigger, Persistables.NotificationTrigger>, INotificationTriggerMetaRepository
{
    public NotificationTriggerMetaRepository(IMapper mapper, IMetaDbContext dbContext, ITenantableRepositoryHook<Public.NotificationTrigger, Persistables.NotificationTrigger>? hook = null) 
        : base(mapper, dbContext, hook) { }

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
