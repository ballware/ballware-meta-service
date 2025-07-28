using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class TenantRepository : TenantBaseRepository
{
    private MetaDbContext MetaContext { get; }
    
    public TenantRepository(IMapper mapper, MetaDbContext dbContext,
        IRepositoryHook<Public.Tenant, Persistables.Tenant>? hook = null) : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }
    
    public override async Task<IEnumerable<TenantSelectListEntry>> AllowedTenantsAsync(IDictionary<string, object> claims)
    {
        var queryParams = new Dictionary<string, object>();

        foreach (var claim in claims)
        {
            queryParams.Add($"claim_{claim.Key}", claim.Value is string[]? $"|{string.Join("|", claim.Value as string[] ?? Array.Empty<string>())}|" : claim.Value);
        }

        return await MetaContext.Database.GetDbConnection().QueryAsync<TenantSelectListEntry>(
            "select Uuid as Id, Name from Tenant where @claim_allowed_tenant like concat('%', Uuid, '%')", queryParams);
    }
}