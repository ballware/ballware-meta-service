using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class LookupMetaRepository : ILookupMetaRepository
{
    private MetaDbContext DbContext { get; }
    
    public LookupMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<IEnumerable<Lookup>> AllForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(DbContext.Lookups.Where(d => d.TenantId == tenantId).OrderBy(c => c.Name));
    }
    
    public virtual async Task<Lookup?> ByIdAsync(Guid tenantId, Guid id)
    {
        return await DbContext.Lookups.SingleOrDefaultAsync(e =>
            e.TenantId == tenantId && e.Uuid == id);
    }
    
    public virtual async Task<Lookup?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        return await DbContext.Lookups.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Identifier == identifier);
    }
}