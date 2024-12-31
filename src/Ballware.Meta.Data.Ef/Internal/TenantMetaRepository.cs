using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class TenantMetaRepository : ITenantMetaRepository
{
    private MetaDbContext DbContext { get; }
    
    public TenantMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<Tenant?> ByIdAsync(Guid id)
    {
        return await DbContext.Tenants.SingleOrDefaultAsync(t => t.Uuid == id);
    }
    
    public virtual async Task<IEnumerable<TenantSelectListEntry>> AllowedTenantsAsync(Dictionary<string, object> claims)
    {
        var queryParams = new Dictionary<string, object>();

        foreach (var claim in claims)
        {
            queryParams.Add($"claim_{claim.Key}", claim.Value is string[]? $"|{string.Join("|", claim.Value as string[] ?? Array.Empty<string>())}|" : claim.Value);
        }
        
        return await DbContext.Database.GetDbConnection().QueryAsync<TenantSelectListEntry>(
            "select Uuid as Id, Name from Tenant where @claim_allowed_tenant like concat('%', Uuid, '%')", queryParams);
    }
}