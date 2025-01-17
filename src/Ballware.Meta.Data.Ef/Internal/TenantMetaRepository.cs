using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class TenantMetaRepository : BaseRepository<Public.Tenant, Persistables.Tenant>, ITenantMetaRepository
{
    public TenantMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) {}
    
    public virtual async Task<Public.Tenant?> ByIdAsync(Guid id)
    {
        var result = await Context.Tenants.SingleOrDefaultAsync(t => t.Uuid == id);
        
        return result != null ? Mapper.Map<Public.Tenant>(result) : null;
    }
    
    public virtual async Task<IEnumerable<TenantSelectListEntry>> AllowedTenantsAsync(Dictionary<string, object> claims)
    {
        var queryParams = new Dictionary<string, object>();

        foreach (var claim in claims)
        {
            queryParams.Add($"claim_{claim.Key}", claim.Value is string[]? $"|{string.Join("|", claim.Value as string[] ?? Array.Empty<string>())}|" : claim.Value);
        }
        
        return await Context.Database.GetDbConnection().QueryAsync<TenantSelectListEntry>(
            "select Uuid as Id, Name from Tenant where @claim_allowed_tenant like concat('%', Uuid, '%')", queryParams);
    }
}