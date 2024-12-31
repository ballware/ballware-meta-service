namespace Ballware.Meta.Data.Repository;

public interface INotificationMetaRepository
{
    Task<Notification?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id);
    Task<Notification?> MetadataByTenantAndIdentifierAsync(Guid tenantId, string identifier);
}