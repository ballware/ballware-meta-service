namespace Ballware.Meta.Data.Repository;

public interface INotificationTriggerMetaRepository
{
    Task<NotificationTrigger> NewAsync(Guid tenantId, Guid notificationId, string notificationParams, Guid? userId);
    
    Task SaveAsync(Guid tenantId, NotificationTrigger trigger, Guid? userId);
}
