using MapsterMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class PickvalueRepository : PickvalueBaseRepository
{
    public PickvalueRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Pickvalue, Persistables.Pickvalue>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }
    
    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Entity, Field, Value, Text, Sorting from Pickvalue where TenantId='{tenantId}'");
    }

    public override Task<string> GenerateAvailableQueryAsync(Guid tenantId, string entity, string field)
    {
        return Task.FromResult($"select Uuid as Id, Value, Text as Name from Pickvalue where TenantId='{tenantId}' and LOWER(Entity)=LOWER('{entity}') and LOWER(Field)=LOWER('{field}') order by Sorting");
    }
}
