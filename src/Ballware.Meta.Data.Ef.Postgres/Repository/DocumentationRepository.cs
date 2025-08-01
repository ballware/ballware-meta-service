using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

public class DocumentationRepository : DocumentationBaseRepository
{
    public DocumentationRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Documentation, Persistables.Documentation>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"SELECT uuid AS \"Id\", entity as \"Entity\", field as \"Field\" FROM documentation WHERE tenant_id='{tenantId}'");
    }
}
