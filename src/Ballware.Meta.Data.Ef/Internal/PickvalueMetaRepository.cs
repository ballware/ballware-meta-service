using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class PickvalueMetaRepository : IPickvalueMetaRepository
{
    private MetaDbContext DbContext { get; }

    public PickvalueMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<IEnumerable<PickvalueSelectEntry>> SelectListForEntityFieldAsync(Guid tenantId, string entity, string field)
    {
        return await Task.FromResult(DbContext.Pickvalues
            .Where(p => p.TenantId == tenantId && p.Entity == entity && p.Field == field)
            .OrderBy(p => p.Sorting)
            .Select(p => new PickvalueSelectEntry { Id = p.Uuid, Name = p.Text, Value = p.Value })
        );
    }

    public async Task<PickvalueSelectEntry?> SelectByValueAsync(Guid tenantId, string entity, string field, int value)
    {
        return await Task.FromResult(DbContext.Pickvalues.SingleOrDefault(p =>
                p.TenantId == tenantId && p.Entity == entity && p.Field == field && p.Value == value)
            .As(p => p != null ? new PickvalueSelectEntry { Id = p.Uuid, Name = p.Text, Value = p.Value } : null));
        
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Entity, Field, Value, Text, Sorting from Pickvalue where TenantId='{tenantId}'");
    }
}
