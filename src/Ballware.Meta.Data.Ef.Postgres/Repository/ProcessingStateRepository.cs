using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

public class ProcessingStateRepository : ProcessingStateBaseRepository
{
    public ProcessingStateRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.ProcessingState, Persistables.ProcessingState>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }
    
    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"SELECT entity AS \"Entity\", state AS \"State\", name AS \"Name\" FROM processing_state WHERE tenant_id='{tenantId}'");
    }
    
    public override Task<string> GenerateAvailableQueryAsync(Guid tenantId, string entity)
    {
        return Task.FromResult($"SELECT uuid AS \"Id\", state AS \"State\", name AS \"Name\" FROM processing_state WHERE tenant_id='{tenantId}' AND entity='{entity}' ORDER BY state");
    }
}
