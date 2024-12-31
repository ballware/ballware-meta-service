using Ballware.Meta.Data.SelectLists;

namespace Ballware.Meta.Data.Repository;

public interface IDocumentMetaRepository
{
    Task<Document> NewAsync(Guid tenantId, Guid? userId);

    Task SaveAsync(Guid tenantId, Document document, Guid? userId);
    
    Task<Document?> MetadataByTenantAndIdAsync(Guid tenantId, Guid id);

    Task<IEnumerable<DocumentSelectListEntry>> SelectListForTenantAsync(Guid tenantId);

    Task<IEnumerable<DocumentSelectListEntry>> SelectListForTenantAndEntityAsync(Guid tenantId, string entity);
}