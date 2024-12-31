using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class ExportMetaRepository : IExportMetaRepository
{
    private MetaDbContext DbContext { get; }

    public ExportMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<Export?> ByIdAsync(Guid id)
    {
        return await DbContext.Exports.SingleOrDefaultAsync(e => e.Uuid == id);
    }

    public virtual async Task<Export> NewAsync(Guid tenantId, Guid? userId)
    {
        return await Task.FromResult(new Export()
        {
            TenantId = tenantId,
            Uuid = Guid.NewGuid(),
            CreatorId = userId,
            CreateStamp = DateTime.Now
        });
    }
    
    public async Task SaveAsync(Guid tenantId, Export export, Guid? userId)
    {
        var existing = await DbContext.Exports.SingleOrDefaultAsync(t => t.TenantId == tenantId && t.Uuid == export.Uuid);

        if (existing == null)
        {
            export.TenantId = tenantId;
            export.CreatorId = userId;
            export.CreateStamp = DateTime.Now;
            
            existing = DbContext.Exports.Add(export).Entity;
        }

        existing.Application = export.Application;
        existing.Entity = export.Entity;
        existing.Query = export.Query;
        existing.MediaType = export.MediaType;
        existing.ExpirationStamp = export.ExpirationStamp;
        existing.LastChangerId = userId;
        existing.LastChangeStamp = DateTime.Now;
        
        DbContext.Update(existing);
        
        await DbContext.SaveChangesAsync();
    }
}
