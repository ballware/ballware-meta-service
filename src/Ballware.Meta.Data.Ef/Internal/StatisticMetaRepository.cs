using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class StatisticMetaRepository : TenantableBaseRepository<Public.Statistic, Persistables.Statistic>, IStatisticMetaRepository
{
    public StatisticMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.Statistic, Persistables.Statistic>? hook = null) 
        : base(mapper, dbContext, hook) { }

    public virtual async Task<Public.Statistic?> MetadataByIdentifierAsync(Guid tenantId, string identifier)
    {
        var result = await Context.Statistics.SingleOrDefaultAsync(d => d.TenantId == tenantId && d.Identifier == identifier);

        return result != null ? Mapper.Map<Public.Statistic>(result) : null;
    }
    
    public virtual async Task<IEnumerable<StatisticSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Statistics.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Identifier)
            .Select(r => new StatisticSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.Name }));
    }
    
    public virtual async Task<StatisticSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.Statistics.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new StatisticSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier, Name = r.Name })
            .FirstOrDefaultAsync();
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Identifier, Name from Statistic where TenantId='{tenantId}'");
    }
}