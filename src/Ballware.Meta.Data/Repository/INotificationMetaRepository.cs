namespace Ballware.Meta.Data.Repository;

public interface INotificationMetaRepository : ITenantableRepository<Public.Notification>
{
    Task<Public.Notification?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id);
    Task<Public.Notification?> MetadataByTenantAndIdentifierAsync(Guid tenantId, string identifier);
}