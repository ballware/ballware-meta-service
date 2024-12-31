namespace Ballware.Meta.Data.Repository;

public interface IExportMetaRepository
{
    Task<Export?> ByIdAsync(Guid id);

    Task SaveAsync(Guid tenantId, Export export, Guid? userId);
    
    Task<Export> NewAsync(Guid tenantId, Guid? userId);
}