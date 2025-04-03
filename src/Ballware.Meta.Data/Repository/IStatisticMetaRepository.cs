using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IStatisticMetaRepository : ITenantableRepository<Public.Statistic>
{
    Task<Public.Statistic?> MetadataByIdentifierAsync(Guid tenantId, string identifier);
    
    Task<IEnumerable<StatisticSelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<StatisticSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
}