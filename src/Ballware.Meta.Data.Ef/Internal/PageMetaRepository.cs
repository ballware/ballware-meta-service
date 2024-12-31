using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class PageMetaRepository : IPageMetaRepository
{
    private MetaDbContext DbContext { get; }
    
    public PageMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<Page?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        return await DbContext.Pages.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Identifier == identifier);
    }
}