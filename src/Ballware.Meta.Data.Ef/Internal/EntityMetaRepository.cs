using System.Data;
using System.Data.Common;
using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class EntityMetaRepository : TenantableBaseRepository<Public.EntityMetadata, Persistables.EntityMetadata>, IEntityMetaRepository
{
    public EntityMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) { }

    public virtual async Task<Public.EntityMetadata?> ByIdAsync(Guid tenantId, Guid id)
    {
        var result = await Context.Entities.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Uuid == id);

        return result != null ? Mapper.Map<Public.EntityMetadata>(result) : null;
    }

    public virtual async Task<Public.EntityMetadata?> ByEntityAsync(Guid tenantId, string entity)
    {
        var result = await Context.Entities.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Entity == entity);

        return result != null ? Mapper.Map<Public.EntityMetadata>(result) : null;
    }

    public Public.EntityMetadata? ByEntity(IDbConnection connection, Guid tenantId, string entity)
    {
        Context.Database.SetDbConnection(connection as DbConnection);

        var result = Context.Entities.SingleOrDefault(e => e.TenantId == tenantId && e.Entity == entity);

        return result != null ? Mapper.Map<Public.EntityMetadata>(result) : null;
    }

    public Public.EntityMetadata? ById(Guid tenantId, Guid id)
    {
        var result = Context.Entities.SingleOrDefault(e => e.TenantId == tenantId && e.Uuid == id);

        return result != null ? Mapper.Map<Public.EntityMetadata>(result) : null;
    }

    public Public.EntityMetadata? ById(IDbConnection connection, Guid tenantId, Guid id)
    {
        Context.Database.SetDbConnection(connection as DbConnection);

        var result = Context.Entities.SingleOrDefault(e => e.TenantId == tenantId && e.Uuid == id);

        return result != null ? Mapper.Map<Public.EntityMetadata>(result) : null;
    }

    public virtual async Task<IEnumerable<EntityRightSelectListEntry>> SelectListEntityRightsForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.EntityRights.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Container).ThenBy(r => r.Identifier)
            .Select(r => new EntityRightSelectListEntry
            { Id = r.Uuid, Identifier = r.Identifier, Name = r.DisplayName, Container = r.Container }));
    }
    
    public virtual async Task<EntityRightSelectListEntry?> SelectEntityRightByIdentifierForTenantAsync(Guid tenantId, string identifier)
    {
        return await Context.EntityRights.Where(r => r.TenantId == tenantId && r.Identifier == identifier)
            .Select(r => new EntityRightSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.DisplayName, Container = r.Container })
            .FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<Public.CharacteristicAssociation>> CharacteristicAssociationsAsync(Guid tenantId, Guid id)
    {
        var entityMetadata = await ByIdAsync(tenantId, id);

        if (entityMetadata != null)
        {
            return await Task.FromResult(Context.CharacteristicAssociations.Where(c =>
                c.TenantId == tenantId && c.Entity == entityMetadata.Entity).Select(c => Mapper.Map<Public.CharacteristicAssociation>(c)));
        }

        return Array.Empty<Public.CharacteristicAssociation>();
    }

    public virtual async Task<IEnumerable<Public.CharacteristicAssociation>> CharacteristicAssociationsAsync(IDbConnection connection, Guid tenantId, Guid id)
    {
        Context.Database.SetDbConnection(connection as DbConnection);

        var entityMetadata = await ByIdAsync(tenantId, id);

        if (entityMetadata != null)
        {
            return await Task.FromResult(Context.CharacteristicAssociations.Where(c =>
                c.TenantId == tenantId && c.Entity == entityMetadata.Entity).Select(c => Mapper.Map<Public.CharacteristicAssociation>(c)));
        }

        return Array.Empty<Public.CharacteristicAssociation>();
    }

    public virtual IEnumerable<Public.CharacteristicAssociation> CharacteristicAssociations(IDbConnection connection,
        Guid tenantId, Guid id)
    {
        var entityMetadata = ById(tenantId, id);

        if (entityMetadata != null)
        {
            return Context.CharacteristicAssociations.Where(c =>
                c.TenantId == tenantId && c.Entity == entityMetadata.Entity)
                .Select(c => Mapper.Map<Public.CharacteristicAssociation>(c));
        }

        return Array.Empty<Public.CharacteristicAssociation>();
    }
    
    public virtual async Task<IEnumerable<EntitySelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Entities.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Entity)
            .Select(r => new EntitySelectListEntry
                { Id = r.Uuid, Entity = r.Entity, Name = r.DisplayName }));
    }
    
    public virtual async Task<EntitySelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.Entities.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new EntitySelectListEntry
                { Id = r.Uuid, Entity = r.Entity, Name = r.DisplayName })
            .FirstOrDefaultAsync();
    }
    
    public virtual async Task<EntitySelectListEntry?> SelectByIdentifierForTenantAsync(Guid tenantId, string identifier)
    {
        return await Context.Entities.Where(r => r.TenantId == tenantId && r.Entity == identifier)
            .Select(r => new EntitySelectListEntry
                { Id = r.Uuid, Entity = r.Entity, Name = r.DisplayName })
            .FirstOrDefaultAsync();
    }
}