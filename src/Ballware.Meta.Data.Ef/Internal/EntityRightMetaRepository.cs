using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class EntityRightMetaRepository : TenantableBaseRepository<Public.EntityRight, Persistables.EntityRight>, IEntityRightMetaRepository
{
    public EntityRightMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.EntityRight, Persistables.EntityRight>? hook = null) 
        : base(mapper, dbContext, hook) { }
    
    public virtual async Task<IEnumerable<EntityRightSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.EntityRights.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Identifier)
            .Select(r => new EntityRightSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.DisplayName, Container = r.Container }));
    }
    
    public virtual async Task<EntityRightSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.EntityRights.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new EntityRightSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.DisplayName, Container = r.Container })
            .FirstOrDefaultAsync();
    }
}