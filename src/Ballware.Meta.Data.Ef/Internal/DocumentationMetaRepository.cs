using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class DocumentationMetaRepository : TenantableRepository<Public.Documentation, Persistables.Documentation>, IDocumentationMetaRepository
{
    private IMetaDbContext MetaContext { get; }

    public DocumentationMetaRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Documentation, Persistables.Documentation>? hook = null)
        : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }

    public virtual async Task<Public.Documentation?> ByEntityAndFieldAsync(Guid tenantId, string entity, string field)
    {
        var result = await MetaContext.Documentations.SingleOrDefaultAsync(e =>
            e.TenantId == tenantId && e.Entity == entity && e.Field == field);

        return result != null ? Mapper.Map<Public.Documentation>(result) : null;
    }
    
    public virtual async Task<IEnumerable<DocumentationSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(MetaContext.Documentations
            .Where(p => p.TenantId == tenantId)
            .OrderBy(c => c.Entity).ThenBy(c => c.Field)
            .Select(d => new DocumentationSelectListEntry { Id = d.Uuid, Entity = d.Entity, Field = d.Field }));
    }
    
    public virtual async Task<DocumentationSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await MetaContext.Documentations.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(d => new DocumentationSelectListEntry { Id = d.Uuid, Entity = d.Entity, Field = d.Field })
            .FirstOrDefaultAsync();
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Entity, Field from Documentation where TenantId='{tenantId}'");
    }
}

