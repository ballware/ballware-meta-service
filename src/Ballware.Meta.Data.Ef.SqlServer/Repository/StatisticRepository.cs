using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class StatisticRepository : StatisticBaseRepository
{
    public StatisticRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Statistic, Persistables.Statistic>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Identifier, Name from Statistic where TenantId='{tenantId}'");
    }
}