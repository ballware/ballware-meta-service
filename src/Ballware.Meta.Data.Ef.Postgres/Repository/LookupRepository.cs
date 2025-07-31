using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

public class LookupRepository : LookupBaseRepository
{
    public LookupRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Lookup, Persistables.Lookup>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"SELECT uuid AS \"Id\", identifier as \"Identifier\", name as \"Name\" FROM lookup WHERE tenant_id='{tenantId}'");
    }
}
