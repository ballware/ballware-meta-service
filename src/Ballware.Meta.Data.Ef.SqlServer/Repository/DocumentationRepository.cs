using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class DocumentationRepository : DocumentationBaseRepository
{
    public DocumentationRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Documentation, Persistables.Documentation>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Entity, Field from Documentation where TenantId='{tenantId}'");
    }
}

