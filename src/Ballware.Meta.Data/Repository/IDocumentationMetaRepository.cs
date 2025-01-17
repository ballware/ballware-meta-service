namespace Ballware.Meta.Data.Repository;

public interface IDocumentationMetaRepository : ITenantableRepository<Public.Documentation>
{
    Task<Public.Documentation?> ByEntityAndFieldAsync(Guid tenantId, string entity, string field);
}