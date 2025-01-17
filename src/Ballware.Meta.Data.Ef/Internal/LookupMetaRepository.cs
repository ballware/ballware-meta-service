using AutoMapper;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class LookupMetaRepository : TenantableBaseRepository<Public.Lookup, Persistables.Lookup>, ILookupMetaRepository
{
    public LookupMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) { }
    
    public virtual async Task<IEnumerable<Public.Lookup>> AllForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Lookups.Where(d => d.TenantId == tenantId)
            .OrderBy(c => c.Name)
            .Select(l => Mapper.Map<Public.Lookup>(l)));
    }
    
    public virtual async Task<Public.Lookup?> ByIdAsync(Guid tenantId, Guid id)
    {
        var result = await Context.Lookups.SingleOrDefaultAsync(e =>
            e.TenantId == tenantId && e.Uuid == id);
        
        return result != null ? Mapper.Map<Public.Lookup>(result) : null;
    }
    
    public virtual async Task<Public.Lookup?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        var result= await Context.Lookups.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Identifier == identifier);
        
        return result != null ? Mapper.Map<Public.Lookup>(result) : null;
    }
}