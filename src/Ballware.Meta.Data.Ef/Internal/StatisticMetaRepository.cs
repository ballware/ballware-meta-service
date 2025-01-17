using AutoMapper;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class StatisticMetaRepository : TenantableBaseRepository<Public.Statistic, Persistables.Statistic>, IStatisticMetaRepository
{
    public StatisticMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) { }

    public virtual async Task<Public.Statistic?> MetadataByIdentifierAsync(Guid tenantId, string identifier)
    {
        var result = await Context.Statistics.SingleOrDefaultAsync(d => d.TenantId == tenantId && d.Identifier == identifier);

        return result != null ? Mapper.Map<Public.Statistic>(result) : null;
    }
}