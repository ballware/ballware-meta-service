using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

public class StatisticRepository : StatisticBaseRepository
{
    public StatisticRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Statistic, Persistables.Statistic>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"SELECT uuid AS \"Id\", identifier AS \"Identifier\", name AS \"Name\" FROM statistic WHERE tenant_id='{tenantId}'");
    }
}
