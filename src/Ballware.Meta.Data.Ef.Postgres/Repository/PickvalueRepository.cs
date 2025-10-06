using AutoMapper;
using Ballware.Meta.Data.Ef.Internal;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Pickvalue = Ballware.Meta.Data.Persistables.Pickvalue;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

public class PickvalueRepository : PickvalueBaseRepository
{
    public PickvalueRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Pickvalue, Persistables.Pickvalue>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }
    
    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"SELECT entity as \"Entity\", field as \"Field\", value as \"Value\", text as \"Text\", sorting as \"Sorting\" FROM pickvalue WHERE tenant_id='{tenantId}'");
    }

    public override Task<string> GenerateAvailableQueryAsync(Guid tenantId, string entity, string field)
    {
        return Task.FromResult($"SELECT uuid AS \"Id\", value AS \"Value\", text AS \"Name\" FROM pickvalue WHERE tenant_id='{tenantId}' AND LOWER(entity)=LOWER('{entity}') AND LOWER(field)=LOWER('{field}') ORDER BY sorting");
    }
}
