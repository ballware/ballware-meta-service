using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class DocumentationMetaRepository : TenantableBaseRepository<Public.Documentation, Persistables.Documentation>, IDocumentationMetaRepository
{
    public DocumentationMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.Documentation, Persistables.Documentation>? hook = null) 
        : base(mapper, dbContext, hook) { }

    public virtual async Task<Public.Documentation?> ByEntityAndFieldAsync(Guid tenantId, string entity, string field)
    {
        var result = await Context.Documentations.SingleOrDefaultAsync(e =>
            e.TenantId == tenantId && e.Entity == entity && e.Field == field);

        return result != null ? Mapper.Map<Public.Documentation>(result) : null;
    }
    
    public virtual async Task<IEnumerable<DocumentationSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Documentations
            .Where(p => p.TenantId == tenantId)
            .OrderBy(c => c.Entity).ThenBy(c => c.Field)
            .Select(d => new DocumentationSelectListEntry { Id = d.Uuid, Entity = d.Entity, Field = d.Field }));
    }
    
    public virtual async Task<DocumentationSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.Documentations.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(d => new DocumentationSelectListEntry { Id = d.Uuid, Entity = d.Entity, Field = d.Field })
            .FirstOrDefaultAsync();
    }
}

