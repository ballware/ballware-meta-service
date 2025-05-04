using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class TenantMetaRepository : BaseRepository<Public.Tenant, Persistables.Tenant>, ITenantMetaRepository
{
    public TenantMetaRepository(IMapper mapper, MetaDbContext dbContext,
        IRepositoryHook<Public.Tenant, Persistables.Tenant>? hook = null) : base(mapper, dbContext, hook)
    {
        
    }

    public virtual async Task<Public.Tenant?> ByIdAsync(Guid id)
    {
        var result = await Context.Tenants.SingleOrDefaultAsync(t => t.Uuid == id);

        IEnumerable<Public.TenantDatabaseObject>? databaseObjects = null;
        
        if (result != null)
        {
            databaseObjects = await this.DatabaseObjectsByTenantAsync(result.Uuid);
        }
        
        return result != null ? Mapper.Map<Public.Tenant>(result, opts =>
        {
            opts.Items["DatabaseObjects"] = databaseObjects ?? [];
        }) : null;
    }

    public virtual async Task<IEnumerable<Public.TenantDatabaseObject>> DatabaseObjectsByTenantAsync(Guid tenant)
    {
        var results = Context.TenantDatabaseObjects.Where(o => o.TenantId == tenant);
        
        return Mapper.Map<IEnumerable<Public.TenantDatabaseObject>>(results);
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
    
    public virtual async Task<IEnumerable<TenantSelectListEntry>> SelectListAsync()
    {
        return await Task.FromResult(Context.Tenants
            .OrderBy(d => d.Name)
            .Select(d => new TenantSelectListEntry { Id = d.Uuid, Name = d.Name }));
    }
    
    public virtual async Task<TenantSelectListEntry?> SelectByIdAsync(Guid id)
    {
        return await Context.Tenants.Where(r => r.Uuid == id)
            .Select(d => new TenantSelectListEntry { Id = d.Uuid, Name = d.Name })
            .FirstOrDefaultAsync();
    }
    
    public Task<string> GenerateListQueryAsync()
    {
        return Task.FromResult($"select Uuid as Id, Name from Tenant");
    }
}