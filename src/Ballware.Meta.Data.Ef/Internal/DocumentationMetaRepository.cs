using AutoMapper;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class DocumentationMetaRepository : TenantableBaseRepository<Public.Documentation, Persistables.Documentation>, IDocumentationMetaRepository
{
    public DocumentationMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) { }
  
    public virtual async Task<Public.Documentation?> ByEntityAndFieldAsync(Guid tenantId, string entity, string field)
    {
        var result= await Context.Documentations.SingleOrDefaultAsync(e =>
            e.TenantId == tenantId && e.Entity == entity && e.Field == field);
        
        return result != null ? Mapper.Map<Public.Documentation>(result) : null;
    }
}

