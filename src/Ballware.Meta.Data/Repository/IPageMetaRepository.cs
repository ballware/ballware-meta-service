namespace Ballware.Meta.Data.Repository;

public interface IPageMetaRepository : ITenantableRepository<Public.Page>
{
    Task<Public.Page?> ByIdentifierAsync(Guid tenantId, string identifier);
}