using AutoMapper;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;
using Pickvalue = Ballware.Meta.Data.Persistables.Pickvalue;

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

            query = query.Where(er => er.Entity == entity.ToString())
                .OrderBy(er => er.Field)
                .ThenBy(er => er.Value);
        }

        if ("entityandfield".Equals(identifier, StringComparison.CurrentCultureIgnoreCase))
        {
            if (!queryParams.TryGetValue("entity", out var entity)) 
            {
                throw new ArgumentException("Entity parameter is required");
            }

            if (!queryParams.TryGetValue("field", out var field)) 
            {
                throw new ArgumentException("Field parameter is required");
            }

            query = query.Where(er => er.Entity == entity.ToString() && er.Field == field.ToString())
                .OrderBy(er => er.Value);
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

    public async Task<IEnumerable<PickvalueAvailability>> GetPickvalueAvailabilityAsync(Guid tenantId)
    {
        return await Task.Run(() => Context.Pickvalues
            .Where(p => p.TenantId == tenantId)
            .Select(p => new { p.Entity, p.Field })
            .Distinct()
            .OrderBy(p => p.Entity)
            .ThenBy(p => p.Field)
            .Select(p => new PickvalueAvailability { Entity = p.Entity, Field = p.Field })
            .ToListAsync());
    }

    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Entity, Field, Value, Text, Sorting from Pickvalue where TenantId='{tenantId}'");
    }

    public Task<string> GenerateAvailableQueryAsync(Guid tenantId, string entity, string field)
    {
        return Task.FromResult($"select Uuid as Id, Value, Text as Name from Pickvalue where TenantId='{tenantId}' and Entity='{entity}' and Field='{field}' order by Sorting");
    }
}
