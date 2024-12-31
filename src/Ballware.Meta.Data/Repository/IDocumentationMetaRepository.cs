namespace Ballware.Meta.Data.Repository;

public interface IDocumentationMetaRepository
{
    Task<Documentation?> ByEntityAndFieldAsync(Guid tenantId, string entity, string field);
}