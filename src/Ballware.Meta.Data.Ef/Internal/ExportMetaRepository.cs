using AutoMapper;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class ExportMetaRepository : TenantableBaseRepository<Public.Export, Persistables.Export>, IExportMetaRepository
{
    public ExportMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) { }

    public async Task<Public.Export?> ByIdAsync(Guid id)
    {
        var result = await Context.Exports.SingleOrDefaultAsync(e => e.Uuid == id);

        return result != null ? Mapper.Map<Public.Export>(result) : null;
    }

}
