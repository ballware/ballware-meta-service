using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class NotificationMetaRepository : TenantableBaseRepository<Public.Notification, Persistables.Notification>, INotificationMetaRepository
{
    public NotificationMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.Notification, Persistables.Notification>? hook = null) 
        : base(mapper, dbContext, hook) { }

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
    
    public virtual async Task<IEnumerable<NotificationSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Notifications.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Identifier)
            .Select(r => new NotificationSelectListEntry
                { Id = r.Uuid, Name = r.Name }));
    }
    
    public virtual async Task<NotificationSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.Notifications.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new NotificationSelectListEntry
                { Id = r.Uuid, Name = r.Name })
            .FirstOrDefaultAsync();
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Name from Notification where TenantId='{tenantId}'");
    }
}
