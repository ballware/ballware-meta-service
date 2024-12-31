using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class StatisticMetaRepository : IStatisticMetaRepository
{
    private MetaDbContext DbContext { get; }
    
    public StatisticMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<Statistic?> MetadataByIdentifierAsync(Guid tenantId, string identifier)
    {
        return await DbContext.Statistics.SingleOrDefaultAsync(d => d.TenantId == tenantId && d.Identifier == identifier);
    }
}