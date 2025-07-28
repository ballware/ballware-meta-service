using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class LookupMetaRepository : TenantableRepository<Public.Lookup, Persistables.Lookup>, ILookupMetaRepository
{
    private IMetaDbContext MetaContext { get; }

    public LookupMetaRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Lookup, Persistables.Lookup>? hook = null)
        : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }

    public virtual async Task<IEnumerable<Public.Lookup>> AllForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(MetaContext.Lookups.Where(d => d.TenantId == tenantId)
            .OrderBy(c => c.Name)
            .Select(l => Mapper.Map<Public.Lookup>(l)));
    }

    public virtual async Task<Public.Lookup?> ByIdAsync(Guid tenantId, Guid id)
    {
        var result = await MetaContext.Lookups.SingleOrDefaultAsync(e =>
            e.TenantId == tenantId && e.Uuid == id);

        return result != null ? Mapper.Map<Public.Lookup>(result) : null;
    }

    public virtual async Task<Public.Lookup?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        var result = await MetaContext.Lookups.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Identifier == identifier);

        return result != null ? Mapper.Map<Public.Lookup>(result) : null;
    }
    
    public virtual async Task<IEnumerable<LookupSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(MetaContext.Lookups.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Identifier)
            .Select(r => new LookupSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.Name }));
    }
    
    public virtual async Task<LookupSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await MetaContext.Lookups.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new LookupSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.Name })
            .FirstOrDefaultAsync();
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Identifier, Name from Lookup where TenantId='{tenantId}'");
    }
}