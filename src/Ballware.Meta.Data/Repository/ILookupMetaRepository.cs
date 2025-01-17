namespace Ballware.Meta.Data.Repository;

public interface ILookupMetaRepository : ITenantableRepository<Public.Lookup>
{
    Task<IEnumerable<Public.Lookup>> AllForTenantAsync(Guid tenantId);

    Task<Public.Lookup?> ByIdAsync(Guid tenantId, Guid id);

    Task<Public.Lookup?> ByIdentifierAsync(Guid tenantId, string identifier);
}