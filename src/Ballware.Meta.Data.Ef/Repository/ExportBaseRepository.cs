using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Repository;

public class ExportBaseRepository : TenantableRepository<Public.Export, Persistables.Export>, IExportMetaRepository
{
    private IMetaDbContext MetaContext { get; }

    public ExportBaseRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Export, Persistables.Export>? hook = null)
        : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }

    public async Task<Public.Export?> ByIdAsync(Guid tenantId, Guid id)
    {
        var result = await MetaContext.Exports.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Uuid == id);

        return result != null ? Mapper.Map<Public.Export>(result) : null;
    }

}
