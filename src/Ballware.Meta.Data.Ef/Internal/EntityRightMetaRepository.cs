using AutoMapper;
using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class EntityRightMetaRepository : TenantableBaseRepository<Public.EntityRight, Persistables.EntityRight>, IEntityRightMetaRepository
{
    public EntityRightMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.EntityRight, Persistables.EntityRight>? hook = null) 
        : base(mapper, dbContext, hook) { }

    protected override IQueryable<EntityRight> ListQuery(IQueryable<EntityRight> query, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        if ("entity".Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!queryParams.TryGetValue("entity", out var entity)) 
            {
                throw new ArgumentException("Entity parameter is required");
            }

            return query.Where(er => er.Entity == entity.ToString());
        }
        
        return base.ListQuery(query, identifier, claims, queryParams);
    }
    
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