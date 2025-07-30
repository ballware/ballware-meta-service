using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Repository;

public interface IDocumentationMetaRepository : ITenantableRepository<Public.Documentation>
{
    Task<Public.Documentation?> ByEntityAndFieldAsync(Guid tenantId, string entity, string field);
    Task<IEnumerable<DocumentationSelectListEntry>> SelectListForTenantAsync(Guid tenantId);
    Task<DocumentationSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id);
    Task<string> GenerateListQueryAsync(Guid tenantId);
}