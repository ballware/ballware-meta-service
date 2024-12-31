using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class NotificationMetaRepository : INotificationMetaRepository
{
    private MetaDbContext DbContext { get; }

    public NotificationMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public async Task<Notification?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id)
    {
        return await DbContext.Notifications.SingleOrDefaultAsync(c => c.TenantId == tenantId && c.Uuid == id);
    }

    public async Task<Notification?> MetadataByTenantAndIdentifierAsync(Guid tenantId, string identifier)
    {
        return await DbContext.Notifications.SingleOrDefaultAsync(c => c.TenantId == tenantId && c.Identifier == identifier);
    }
}
