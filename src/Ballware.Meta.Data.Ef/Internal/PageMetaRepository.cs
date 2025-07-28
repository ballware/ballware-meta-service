using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class PageMetaRepository : TenantableRepository<Public.Page, Persistables.Page>, IPageMetaRepository
{
    private IMetaDbContext MetaContext { get; }

    public PageMetaRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Page, Persistables.Page>? hook = null)
        : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }

    public virtual async Task<Public.Page?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        var result = await MetaContext.Pages.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Identifier == identifier);

        return result != null ? Mapper.Map<Public.Page>(result) : null;
    }
    
    public virtual async Task<IEnumerable<PageSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(MetaContext.Pages.Where(r => r.TenantId == tenantId)
            .OrderBy(r => new { r.Identifier })
            .Select(r => new PageSelectListEntry
                { Id = r.Uuid, Name = r.Name }));
    }
    
    public virtual async Task<PageSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await MetaContext.Pages.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new PageSelectListEntry
                { Id = r.Uuid, Name = r.Name })
            .FirstOrDefaultAsync();
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Name from Page where TenantId='{tenantId}'");
    }
}