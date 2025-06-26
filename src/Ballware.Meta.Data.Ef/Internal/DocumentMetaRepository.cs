using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class DocumentMetaRepository : TenantableBaseRepository<Public.Document, Persistables.Document>, IDocumentMetaRepository
{
    public DocumentMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.Document, Persistables.Document>? hook = null) 
        : base(mapper, dbContext, hook) { }

    public virtual async Task<Public.Document?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id)
    {
        var result = await Context.Documents.SingleOrDefaultAsync(d => d.TenantId == tenantId && d.Uuid == id);

        return result != null ? Mapper.Map<Public.Document>(result) : null;
    }

    public virtual async Task<IEnumerable<DocumentSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Documents
            .Where(p => p.TenantId == tenantId)
            .Select(d => new { d.Uuid, d.DisplayName, d.State, d.ReportParameter })
            .OrderBy(c => c.DisplayName)
            .Select(d => new DocumentSelectListEntry { Id = d.Uuid, Name = d.DisplayName, State = d.State, ReportParameter = d.ReportParameter}));
    }
    
    public virtual async Task<DocumentSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.Documents.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(d => new DocumentSelectListEntry { Id = d.Uuid, Name = d.DisplayName, State = d.State, ReportParameter = d.ReportParameter })
            .FirstOrDefaultAsync();
    }

    public async Task<int?> GetCurrentStateForTenantAndIdAsync(Guid tenantId, Guid id)
    {
        return await Context.Documents
            .Where(d => d.TenantId == tenantId && d.Uuid == id)
            .Select(d => (int?)d.State)
            .FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<DocumentSelectListEntry>> SelectListForTenantAndEntityAsync(Guid tenantId, string entity)
    {
        return await Task.FromResult(Context.Documents
            .Where(p => p.TenantId == tenantId && p.Entity == entity)
            .Select(d => new { d.Uuid, d.DisplayName, d.State })
            .OrderBy(c => c.DisplayName)
            .Select(d => new DocumentSelectListEntry { Id = d.Uuid, Name = d.DisplayName, State = d.State }));
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, DisplayName as Name, State from Document where TenantId='{tenantId}'");
    }
}

