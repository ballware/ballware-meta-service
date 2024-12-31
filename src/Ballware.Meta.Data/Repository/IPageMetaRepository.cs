namespace Ballware.Meta.Data.Repository;

public interface IPageMetaRepository
{
    Task<Page?> ByIdentifierAsync(Guid tenantId, string identifier);
}