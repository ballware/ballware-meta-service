using AutoMapper;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class PageMetaRepository : TenantableBaseRepository<Public.Page, Persistables.Page>, IPageMetaRepository
{
    public PageMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) { }

    public virtual async Task<Public.Page?> ByIdentifierAsync(Guid tenantId, string identifier)
    {
        var result = await Context.Pages.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Identifier == identifier);

        return result != null ? Mapper.Map<Public.Page>(result) : null;
    }
}