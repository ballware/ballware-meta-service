using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class PageMetaRepository : TenantableBaseRepository<Public.Page, Persistables.Page>, IPageMetaRepository
{
    public PageMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.Page, Persistables.Page>? hook = null) 
        : base(mapper, dbContext, hook) { }

    public virtual async Task<Public.Page?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        var result = await Context.Pages.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Identifier == identifier);

        return result != null ? Mapper.Map<Public.Page>(result) : null;
    }
    
    public virtual async Task<IEnumerable<PageSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Pages.Where(r => r.TenantId == tenantId)
            .OrderBy(r => new { r.Identifier })
            .Select(r => new PageSelectListEntry
                { Id = r.Uuid, Name = r.Name }));
    }
    
    public virtual async Task<PageSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.Pages.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new PageSelectListEntry
                { Id = r.Uuid, Name = r.Name })
            .FirstOrDefaultAsync();
    }
}