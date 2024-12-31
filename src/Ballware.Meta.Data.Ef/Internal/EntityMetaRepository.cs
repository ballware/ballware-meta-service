using System.Data;
using System.Data.Common;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class EntityMetaRepository : IEntityMetaRepository
{
    private MetaDbContext DbContext { get; }
        
    public EntityMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<EntityMetadata?> ByIdAsync(Guid tenantId, Guid id)
    {
        return await DbContext.Entities.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Uuid == id);
    }
    
    public virtual async Task<EntityMetadata?> ByEntityAsync(Guid tenantId, string entity)
    {
        return await DbContext.Entities.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Entity == entity);
    }

    public EntityMetadata? ByEntity(IDbConnection connection, Guid tenantId, string entity)
    {
        DbContext.Database.SetDbConnection(connection as DbConnection);

        return DbContext.Entities.SingleOrDefault(e => e.TenantId == tenantId && e.Entity == entity);
    }
    
    public EntityMetadata? ById(Guid tenantId, Guid id)
    {
        return DbContext.Entities.SingleOrDefault(e => e.TenantId == tenantId && e.Uuid == id);
    }
    
    public EntityMetadata? ById(IDbConnection connection, Guid tenantId, Guid id)
    {
        DbContext.Database.SetDbConnection(connection as DbConnection);

        return DbContext.Entities.SingleOrDefault(e => e.TenantId == tenantId && e.Uuid == id);
    }
    
    public virtual async Task<IEnumerable<EntityRightSelectListEntry>> SelectListEntityRightsAsync(Guid tenantId)
    {
        return await Task.FromResult(DbContext.EntityRights.Where(r => r.TenantId == tenantId)
            .OrderBy(r => new { r.Container, r.Identifier })
            .Select(r => new EntityRightSelectListEntry
                { Id = r.Identifier, Name = r.DisplayName, Container = r.Container }));
    }

    public virtual async Task<IEnumerable<CharacteristicAssociation>> CharacteristicAssociationsAsync(Guid tenantId, Guid id)
    {
        var entityMetadata = await ByIdAsync(tenantId, id);

        if (entityMetadata != null)
        {
            return await Task.FromResult(DbContext.CharacteristicAssociations.Where(c =>
                c.TenantId == tenantId && c.Entity == entityMetadata.Entity));    
        }
        
        return Array.Empty<CharacteristicAssociation>();
    }
    
    public virtual async Task<IEnumerable<CharacteristicAssociation>> CharacteristicAssociationsAsync(IDbConnection connection, Guid tenantId, Guid id)
    {
        DbContext.Database.SetDbConnection(connection as DbConnection);
        
        var entityMetadata = await ByIdAsync(tenantId, id);

        if (entityMetadata != null)
        {
            return await Task.FromResult(DbContext.CharacteristicAssociations.Where(c =>
                c.TenantId == tenantId && c.Entity == entityMetadata.Entity));    
        }
     
        return Array.Empty<CharacteristicAssociation>();
    }

    public virtual IEnumerable<CharacteristicAssociation> CharacteristicAssociations(IDbConnection connection,
        Guid tenantId, Guid id)
    {
        var entityMetadata = ById(tenantId, id);

        if (entityMetadata != null)
        {
            return DbContext.CharacteristicAssociations.Where(c =>
                c.TenantId == tenantId && c.Entity == entityMetadata.Entity);    
        }
            
        return Array.Empty<CharacteristicAssociation>();
    }
}