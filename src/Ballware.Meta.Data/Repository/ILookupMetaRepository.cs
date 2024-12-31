namespace Ballware.Meta.Data.Repository;

public interface ILookupMetaRepository
{
    Task<IEnumerable<Lookup>> AllForTenantAsync(Guid tenantId);

    Task<Lookup?> ByIdAsync(Guid tenantId, Guid id);

    Task<Lookup?> ByIdentifierAsync(Guid tenantId, string identifier);
}