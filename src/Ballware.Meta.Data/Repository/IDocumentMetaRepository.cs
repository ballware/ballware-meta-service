using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IDocumentMetaRepository : ITenantableRepository<Public.Document>
{
    Task<Public.Document?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id);

    Task<IEnumerable<DocumentSelectListEntry>> SelectListForTenantAsync(Guid tenantId);

    Task<IEnumerable<DocumentSelectListEntry>> SelectListForTenantAndEntityAsync(Guid tenantId, string entity);
    
    Task<DocumentSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
}