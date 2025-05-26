using AutoMapper;
using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class PickvalueMetaRepository : TenantableBaseRepository<Public.Pickvalue, Persistables.Pickvalue>, IPickvalueMetaRepository
{
    public PickvalueMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.Pickvalue, Persistables.Pickvalue>? hook = null) 
        : base(mapper, dbContext, hook) { }

    protected override IQueryable<Pickvalue> ListQuery(IQueryable<Pickvalue> query, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        if ("entity".Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!queryParams.TryGetValue("entity", out var entity)) 
            {
                throw new ArgumentException("Entity parameter is required");
            }

            return query.Where(er => er.Entity == entity.ToString());
        }
        
        return base.ListQuery(query, identifier, claims, queryParams);
    }

    public async Task<IEnumerable<PickvalueSelectEntry>> SelectListForEntityFieldAsync(Guid tenantId, string entity, string field)
    {
        return await Task.Run(() => Context.Pickvalues
            .Where(p => p.TenantId == tenantId && p.Entity == entity && p.Field == field)
            .OrderBy(p => p.Sorting)
            .Select(p => new PickvalueSelectEntry { Id = p.Uuid, Name = p.Text, Value = p.Value })
        );
    }

    public async Task<PickvalueSelectEntry?> SelectByValueAsync(Guid tenantId, string entity, string field, int value)
    {
        return await Task.Run(() => Context.Pickvalues.SingleOrDefault(p =>
                p.TenantId == tenantId && p.Entity == entity && p.Field == field && p.Value == value)
            .As(p => p != null ? new PickvalueSelectEntry { Id = p.Uuid, Name = p.Text, Value = p.Value } : null));

    }

    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Entity, Field, Value, Text, Sorting from Pickvalue where TenantId='{tenantId}'");
    }
}
