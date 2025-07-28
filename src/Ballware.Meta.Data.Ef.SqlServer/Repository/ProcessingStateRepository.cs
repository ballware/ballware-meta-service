using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class ProcessingStateRepository : ProcessingStateBaseRepository
{
    public ProcessingStateRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.ProcessingState, Persistables.ProcessingState>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }
    
    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Entity, State, Name from ProcessingState where TenantId='{tenantId}'");
    }
    
    public override Task<string> GenerateAvailableQueryAsync(Guid tenantId, string entity)
    {
        return Task.FromResult($"select Uuid as Id, State, Name from ProcessingState where TenantId='{tenantId}' and Entity='{entity}' order by State");
    }
}