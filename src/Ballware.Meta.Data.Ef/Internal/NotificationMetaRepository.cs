using AutoMapper;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class NotificationMetaRepository : TenantableBaseRepository<Public.Notification, Persistables.Notification>, INotificationMetaRepository
{
    public NotificationMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) {}
    
    public async Task<Public.Notification?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id)
    {
        var result = await Context.Notifications.SingleOrDefaultAsync(c => c.TenantId == tenantId && c.Uuid == id);
        
        return result != null ? Mapper.Map<Public.Notification>(result) : null;
    }

    public async Task<Public.Notification?> MetadataByTenantAndIdentifierAsync(Guid tenantId, string identifier)
    {
        var result = await Context.Notifications.SingleOrDefaultAsync(c => c.TenantId == tenantId && c.Identifier == identifier);
        
        return result != null ? Mapper.Map<Public.Notification>(result) : null;
    }
}
