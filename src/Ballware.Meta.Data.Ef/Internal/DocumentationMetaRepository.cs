using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class DocumentationMetaRepository : IDocumentationMetaRepository
{
    private MetaDbContext DbContext { get; }

    public DocumentationMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
  
    public virtual async Task<Documentation?> ByEntityAndFieldAsync(Guid tenantId, string entity, string field)
    {
        return await DbContext.Documentations.SingleOrDefaultAsync(e =>
            e.TenantId == tenantId && e.Entity == entity && e.Field == field);
    }
}

