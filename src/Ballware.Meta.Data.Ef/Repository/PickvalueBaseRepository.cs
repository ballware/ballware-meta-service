using AutoMapper;
using Ballware.Meta.Data.Ef.Internal;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Pickvalue = Ballware.Meta.Data.Persistables.Pickvalue;

namespace Ballware.Meta.Data.Ef.Repository;

public abstract class PickvalueBaseRepository : TenantableRepository<Public.Pickvalue, Persistables.Pickvalue>, IPickvalueMetaRepository
{
    private IMetaDbContext MetaContext { get; }

    public PickvalueBaseRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Pickvalue, Persistables.Pickvalue>? hook = null)
        : base(mapper, dbContext, hook)
    {
        MetaContext = dbContext;
    }

    protected override IQueryable<Pickvalue> ListQuery(IQueryable<Pickvalue> query, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        if ("entity".Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!queryParams.TryGetValue("entity", out var entity)) 
            {
                throw new ArgumentException("Entity parameter is required");
            }

            var entityStr = entity.ToString()!.ToLower();
            query = query.Where(er => er.Entity!.ToLower() == entityStr)
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

            var entityStr = entity.ToString()!.ToLower();
            var fieldStr = field.ToString()!.ToLower();
            query = query.Where(er => er.Entity!.ToLower() == entityStr && er.Field!.ToLower() == fieldStr)
                .OrderBy(er => er.Value);
        }
        
        return base.ListQuery(query, identifier, claims, queryParams);
    }

    public async Task<IEnumerable<PickvalueSelectEntry>> SelectListForEntityFieldAsync(Guid tenantId, string entity, string field)
    {
        var entityLower = entity.ToLower();
        var fieldLower = field.ToLower();
        return await Task.Run(() => MetaContext.Pickvalues
            .Where(p => p.TenantId == tenantId && p.Entity!.ToLower() == entityLower && p.Field!.ToLower() == fieldLower)
            .OrderBy(p => p.Sorting)
            .Select(p => new PickvalueSelectEntry { Id = p.Uuid, Name = p.Text, Value = p.Value })
        );
    }

    public async Task<PickvalueSelectEntry?> SelectByValueAsync(Guid tenantId, string entity, string field, int value)
    {
        var entityLower = entity.ToLower();
        var fieldLower = field.ToLower();
        return await Task.Run(() => MetaContext.Pickvalues.SingleOrDefault(p =>
                p.TenantId == tenantId && p.Entity!.ToLower() == entityLower && p.Field!.ToLower() == fieldLower && p.Value == value)
            .As(p => p != null ? new PickvalueSelectEntry { Id = p.Uuid, Name = p.Text, Value = p.Value } : null));

    }

    public async Task<IEnumerable<PickvalueAvailability>> GetPickvalueAvailabilityAsync(Guid tenantId)
    {
        return await Task.Run(() => MetaContext.Pickvalues
            .Where(p => p.TenantId == tenantId)
            .Select(p => new { p.Entity, p.Field })
            .Distinct()
            .OrderBy(p => p.Entity)
            .ThenBy(p => p.Field)
            .Select(p => new PickvalueAvailability { Entity = p.Entity, Field = p.Field })
            .ToListAsync());
    }

    public abstract Task<string> GenerateListQueryAsync(Guid tenantId);

    public abstract Task<string> GenerateAvailableQueryAsync(Guid tenantId, string entity, string field);
}
