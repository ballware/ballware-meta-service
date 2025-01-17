namespace Ballware.Meta.Data.Repository;

public interface INotificationTriggerMetaRepository : ITenantableRepository<Public.NotificationTrigger>
{
    Task<Public.NotificationTrigger> NewAsync(Guid tenantId, Guid notificationId, string notificationParams, Guid? userId);
}
