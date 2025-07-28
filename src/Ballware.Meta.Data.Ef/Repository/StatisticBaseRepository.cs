using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Repository;

public abstract class StatisticBaseRepository : TenantableRepository<Public.Statistic, Persistables.Statistic>, IStatisticMetaRepository
{
    private IMetaDbContext MetaContext { get; }

    public StatisticBaseRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Statistic, Persistables.Statistic>? hook = null)
        : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }

    public virtual async Task<Public.Statistic?> MetadataByIdentifierAsync(Guid tenantId, string identifier)
    {
        var result = await MetaContext.Statistics.SingleOrDefaultAsync(d => d.TenantId == tenantId && d.Identifier == identifier);

        return result != null ? Mapper.Map<Public.Statistic>(result) : null;
    }
    
    public virtual async Task<IEnumerable<StatisticSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(MetaContext.Statistics.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Identifier)
            .Select(r => new StatisticSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.Name }));
    }
    
    public virtual async Task<StatisticSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await MetaContext.Statistics.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new StatisticSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.Name })
            .FirstOrDefaultAsync();
    }

    public abstract Task<string> GenerateListQueryAsync(Guid tenantId);
}