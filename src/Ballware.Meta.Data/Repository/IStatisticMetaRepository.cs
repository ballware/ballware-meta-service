namespace Ballware.Meta.Data.Repository;

public interface IStatisticMetaRepository : ITenantableRepository<Public.Statistic>
{
    Task<Public.Statistic?> MetadataByIdentifierAsync(Guid tenantId, string identifier);
}