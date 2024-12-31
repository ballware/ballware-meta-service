namespace Ballware.Meta.Data.Repository;

public interface IStatisticMetaRepository
{
    Task<Statistic?> MetadataByIdentifierAsync(Guid tenantId, string identifier);
}