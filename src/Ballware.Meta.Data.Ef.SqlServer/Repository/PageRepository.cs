using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class PageRepository : PageBaseRepository
{
    public PageRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Page, Persistables.Page>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Name from Page where TenantId='{tenantId}'");
    }
}