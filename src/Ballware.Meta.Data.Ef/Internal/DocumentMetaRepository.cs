using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class DocumentMetaRepository : IDocumentMetaRepository
{
    private MetaDbContext DbContext { get; }

    public DocumentMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<Document> NewAsync(Guid tenantId, Guid? userId)
    {
        return await Task.FromResult(new Document()
        {
            TenantId = tenantId,
            Uuid = Guid.NewGuid(),
            CreatorId = userId,
            CreateStamp = DateTime.Now
        });
    }
    
    public async Task SaveAsync(Guid tenantId, Document document, Guid? userId)
    {
        var existing = await DbContext.Documents.SingleOrDefaultAsync(t => t.TenantId == tenantId && t.Uuid == document.Uuid);

        if (existing == null)
        {
            document.TenantId = tenantId;
            document.CreatorId = userId;
            document.CreateStamp = DateTime.Now;
            
            existing = DbContext.Documents.Add(document).Entity;
        }
        
        existing.DisplayName = document.DisplayName;
        existing.Entity = document.Entity;
        existing.State = document.State;
        existing.ReportBinary = document.ReportBinary;
        existing.ReportParameter = document.ReportParameter;
        existing.LastChangerId = userId;
        existing.LastChangeStamp = DateTime.Now;
        
        await DbContext.SaveChangesAsync();
    }
    
    public async Task<Document?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id)
    {
        return await DbContext.Documents.SingleOrDefaultAsync(d => d.TenantId == tenantId && d.Uuid == id);
    }

    public async Task<IEnumerable<DocumentSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(DbContext.Documents
            .Where(p => p.TenantId == tenantId)
            .Select(d => new { d.Uuid, d.DisplayName, d.State })
            .OrderBy(c => c.DisplayName)
            .Select(d => new DocumentSelectListEntry { Id = d.Uuid, Name = d.DisplayName, State = d.State }));
    }

    public async Task<IEnumerable<DocumentSelectListEntry>> SelectListForTenantAndEntityAsync(Guid tenantId, string entity)
    {
        return await Task.FromResult(DbContext.Documents
            .Where(p => p.TenantId == tenantId && p.Entity == entity)
            .Select(d => new { d.Uuid, d.DisplayName, d.State })
            .OrderBy(c => c.DisplayName)
            .Select(d => new DocumentSelectListEntry { Id = d.Uuid, Name = d.DisplayName, State = d.State }));
    }
}

